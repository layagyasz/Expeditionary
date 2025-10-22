using Cardamom.Utils.Suppliers.Promises;

namespace Expeditionary.Loader
{
    public abstract class LoaderTaskNode<T> : ILoaderTask
    {
        public bool IsGL { get; }

        private readonly List<ILoaderTask> _children = new();

        protected readonly RemotePromise<T> _promise = new();

        protected LoaderTaskNode(bool isGL)
        {
            IsGL = isGL;
        }

        public void AddChild(ILoaderTask node)
        {
            _children.Add(node);
        }

        public void DoNow()
        {
            foreach (var parent in GetParents())
            {
                parent.DoNow();
            }
            Perform();
        }

        public T GetNow()
        {
            DoNow();
            return _promise.Get();
        }

        public IPromise<T> GetPromise()
        {
            return _promise;
        }

        public bool IsDone()
        {
            return _promise.HasValue();
        }

        public IEnumerable<ILoaderTask> GetChildren()
        {
            return _children;
        }

        public abstract IEnumerable<ILoaderTask> GetParents();

        public abstract bool IsReady();

        public abstract void Notify(ILoaderTask parent);

        public abstract void Perform();
    }
}
