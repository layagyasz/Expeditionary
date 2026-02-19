using Expeditionary.Controller.Screens;
using Expeditionary.Loader;
using Expeditionary.Model;
using Expeditionary.Runners.Loaders.Runtime;
using Expeditionary.View.Screens;

namespace Expeditionary.Runners.GameStates
{
    public class MainMenuState : IGameState
    {
        public event EventHandler<IGameStateContext>? GameStateChanged;

        private readonly GameModule _module;

        private MainMenuScreen? _screen;
        private MainMenuScreenController? _controller;

        public MainMenuState(GameModule module)
        {
            _module = module;
        }

        public IScreen Enter(object? context, ScreenFactory screenFactory)
        {
            _screen = screenFactory.CreateMainMenu();
            _controller = (MainMenuScreenController)_screen.Controller;
            _controller.MenuClicked += HandleMenuClicked;
            return _screen;
        }

        public void Exit()
        {
            _screen!.Dispose();
            _screen = null;

            _controller!.MenuClicked -= HandleMenuClicked;
            _controller = null;
        }

        private void HandleMenuClicked(object? sender, object e)
        {
            if (e == MainMenuScreen.NewGame)
            {
                (var status, var task) = NewGalaxyLoader.Load(_module, seed: 0);
                GameStateChanged?.Invoke(
                    this,
                    new IGameStateContext.LoadContext(
                        status, 
                        task.Map(result => (IGameStateContext)new IGameStateContext.GalaxyContext(result))));
            }
        }
    }
}
