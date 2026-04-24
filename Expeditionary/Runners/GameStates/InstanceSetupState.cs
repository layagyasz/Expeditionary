using Expeditionary.Controller.Screens;
using Expeditionary.Loader;
using Expeditionary.Model;
using Expeditionary.Runners.Loaders.Runtime;
using Expeditionary.View.Screens;

namespace Expeditionary.Runners.GameStates
{
    public class InstanceSetupState : IGameState
    {
        public event EventHandler<IGameStateContext>? GameStateChanged;

        private readonly GameModule _module;

        private InstanceSetupScreen? _screen;
        private InstanceSetupScreenController? _controller;

        public InstanceSetupState(GameModule module)
        {
            _module = module;
        }

        public IScreen Enter(object? context, ScreenFactory screenFactory)
        {
            _screen = screenFactory.CreateInstanceSetup(_module);
            _controller = (InstanceSetupScreenController)_screen.Controller;
            _controller.Submitted += HandleSubmit;
            return _screen;
        }

        public void Exit()
        {
            _screen!.Dispose();
            _screen = null;

            _controller!.Submitted -= HandleSubmit;
            _controller = null;
        }

        private void HandleSubmit(object? sender, GameInstanceParameters parameters)
        {
            (var status, var task) = NewGameInstanceLoader.Load(_module, parameters);
            GameStateChanged?.Invoke(
                this,
                new IGameStateContext.LoadContext(
                    status,
                    task.Map(result => (IGameStateContext)new IGameStateContext.GalaxyContext(result))));
        }
    }
}
