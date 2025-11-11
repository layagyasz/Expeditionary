using Cardamom.Graphics;
using Cardamom.Ui;
using Expeditionary.Loader;
using Expeditionary.View.Scenes;
using Expeditionary.View.Screens;

namespace Expeditionary.Runners
{
    public class LoadRunner : UiRunner
    {
        public LoadRunner(ProgramConfig config)
            : base(config) { }

        protected override IRenderable MakeRoot(
            ProgramData data, UiElementFactory uiElementFactory, SceneFactory sceneFactory, ThreadedLoader loader)
        {
            var status = new LoaderStatus(Enumerable.Range(0, 10).Cast<object>().ToList(), logLength: 1);
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
            return LoadScreen.Create(uiElementFactory, task, status);
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
