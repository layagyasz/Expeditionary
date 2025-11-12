using Cardamom.Ui;
using Expeditionary.Loader;
using Expeditionary.Runners.Loaders.Runtime;
using Expeditionary.View.Screens;

namespace Expeditionary.Runners
{
    public class GalaxyRunner : UiRunner
    {
        public GalaxyRunner(ProgramConfig config)
            : base(config) { }

        protected override void Handle(
            ProgramData data, UiWindow window, ThreadedLoader loader, ScreenFactory screenFactory)
        {
            var module = data.Module;
            (var _, var task) = NewGalaxyLoader.Load(module, seed: 0);
            window.SetRoot(screenFactory.CreateGalaxy(module, task.GetNow()));
        }
    }
}
