using Cardamom.Ui;
using Expeditionary.Loader;
using Expeditionary.Model;
using Expeditionary.Runners.GameStates;
using Expeditionary.View.Screens;

namespace Expeditionary.Runners
{
    public class DefaultRunner : UiRunner
    {
        private readonly Func<GameModule, IGameStateContext> _initialContextFn;

        public DefaultRunner(ProgramConfig config, Func<GameModule, IGameStateContext> initialContextFn) 
            : base(config) 
        { 
            _initialContextFn = initialContextFn;
        }

        protected override void Handle(
            ProgramData data, 
            UiWindow window, 
            ThreadedLoader loader, 
            ScreenFactory screenFactory) 
        {
            var programController = 
                new ProgramController(data.Config, window, loader, screenFactory, data.Module, data.Localization);
            programController.Initialize();
            programController.Enter(_initialContextFn(data.Module));
        }
    }
}
