using Cardamom;
using Cardamom.Collections;
using Cardamom.Logging;
using Cardamom.Utils.Suppliers.Promises;
using Cardamom.Window;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Expeditionary.Loader
{
    public class ThreadedLoader : ManagedResource
    {
        delegate void QueueCallback(ILoaderTask task);

        interface IWorker : IDisposable
        {
            int GetTaskCount();
            void QueueTask(ILoaderTask task);
            void Start();
        }

        class Worker : IWorker
        {            
            private readonly QueueCallback _queueCallback;
            private readonly Queue<ILoaderTask> _tasks = new();
            private readonly Thread _thread;

            public Worker(QueueCallback queueCallback)
            {
                _queueCallback = queueCallback;
                _thread = new Thread(WorkThread);
            }

            public virtual void Dispose() { }

            public int GetTaskCount()
            {
                return _tasks.Count;
            }

            public void QueueTask(ILoaderTask task)
            {
                Monitor.Enter(_tasks);                
                _tasks.Enqueue(task);
                Monitor.Pulse(_tasks);
                Monitor.Exit(_tasks);
            }

            public void Start()
            {
                _thread.Start();
            }

            private void WorkThread()
            {
                WorkThreadImpl();
            }

            protected virtual void WorkThreadImpl()
            {
                while (true)
                {
                    ILoaderTask task;
                    Monitor.Enter(_tasks);
                    if (_tasks.Count > 0)
                    {
                        task = _tasks.Dequeue();
                    }
                    else
                    {
                        Monitor.Wait(_tasks);
                        task = _tasks.Dequeue();
                    }
                    Monitor.Exit(_tasks);
                    task.Perform();
                    foreach (var child in task.GetChildren())
                    {
                        lock (child)
                        {
                            child.Notify(task);
                            if (child.IsReady())
                            {
                                _queueCallback(child);
                            }
                        }
                    }
                }
            }
        }

        class GLWorker : Worker
        {
            private readonly NativeWindow _window;

            public GLWorker(RenderWindow parentWindow, QueueCallback queueCallback)
                : base(queueCallback)
            {
                _window = new NativeWindow(
                    new NativeWindowSettings()
                    {
                        SharedContext = parentWindow.GetContext(),
                        Flags = ContextFlags.Offscreen,
                        StartVisible = false
                    });
            }

            public override void Dispose()
            {
                _window.Dispose();
            }

            protected override void WorkThreadImpl()
            {
                _window.MakeCurrent();
                base.WorkThreadImpl();
            }
        }

        private readonly ILogger _logger;
        private readonly List<IWorker> _workers = new();
        private readonly List<IWorker> _glWorkers = new();

        public ThreadedLoader(RenderWindow parentWindow, int numWorkers, int numGLWorkers, ILogger logger)
        {
            _logger = logger.ForType(typeof(ThreadedLoader));
            for (int i=0; i<numWorkers; ++i)
            {
                _workers.Add(new Worker(QueueTask));
            }
            for (int i=0; i<numGLWorkers; ++i)
            {
                var worker = new GLWorker(parentWindow, QueueTask);
                _workers.Add(worker);
                _glWorkers.Add(worker);
            }
            parentWindow.MakeCurrent();
        }

        public void QueueTaskTree(ILoaderTask task)
        {
            foreach (var ancestor in GetProgenitors(task))
            {
                QueueTask(ancestor);
            }
        }

        public IPromise<T> Load<T>(LoaderTaskNode<T> root)
        {
            QueueTaskTree(root);
            return root.GetPromise();
        }

        public void Start()
        {
            foreach (var worker in _workers)
            {
                worker.Start();
            }
            _logger.AtInfo().Log($"Started {_workers.Count} workers");
        }

        protected override void DisposeImpl()
        {
            foreach (var worker in _workers)
            {
                worker.Dispose();
            }
            _workers.Clear();
        }

        private IEnumerable<ILoaderTask> GetProgenitors(ILoaderTask task)
        {
            if (!task.GetParents().Any())
            {
                return Enumerable.Repeat(task, 1);
            }
            return task.GetParents().SelectMany(GetProgenitors);
        }

        private void QueueTask(ILoaderTask task)
        {
            if (task.IsGL)
            {
                _glWorkers.ArgMin(x => x.GetTaskCount())!.QueueTask(task);
            }
            else
            {
                _workers.ArgMin(x => x.GetTaskCount())!.QueueTask(task);
            }
        }
    }
}
