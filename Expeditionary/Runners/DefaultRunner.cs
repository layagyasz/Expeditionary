using Cardamom.Ui;
using Expeditionary.Loader;
using Expeditionary.View.Screens;

namespace Expeditionary.Runners
{
    public class DefaultRunner : UiRunner
    {
        public DefaultRunner(ProgramConfig config) 
            : base(config) { }

        protected override void Handle(
            ProgramData data, 
            UiWindow window, 
            ThreadedLoader loader, 
            ScreenFactory screenFactory) 
        {
            var programController = 
                new ProgramController(data.Config, window, loader, screenFactory, data.Module, data.Localization);
            programController.Initialize();
        }
    }
}
