using Cardamom.Ui;
using Expeditionary.Loader;
using Expeditionary.View.Screens;

namespace Expeditionary.Runners
{
    public class LoadRunner : UiRunner
    {
        public LoadRunner(ProgramConfig config)
            : base(config) { }

        protected override void Handle(
            ProgramData data, UiWindow window, ThreadedLoader loader, ScreenFactory screenFactory)
        {
            var status = new LoaderStatus(logLength: 1);
            status.AddSegments(0, 1, 2, 3, 4, 5, 6, 7, 8, 9);
            var tasks = new List<LoaderTaskNode<object>>();
            for (int i=0; i<10; ++i)
            {
                object segment = i;
                tasks.Add(new SourceLoaderTask<object>(() => Load(segment, status), isGL: false));
            }
            var task = 
                AggregateLoaderTask<object, List<object>>.Aggregate(
                    tasks, () => new(), (list, x) => { list.Add(x); return list; });
            loader.Load(task);
            window.SetRoot(screenFactory.CreateLoad(task, status));
        }

        private static object Load(object segment, LoaderStatus status)
        {
            status.AddWork(segment, 1);
            Thread.Sleep(1000);
            status.DoWork(segment);
            return new();
        }
    }
}
