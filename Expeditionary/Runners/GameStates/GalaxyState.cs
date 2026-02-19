using Expeditionary.Controller.Screens;
using Expeditionary.Loader;
using Expeditionary.Model;
using Expeditionary.Model.Missions;
using Expeditionary.Runners.Loaders.Runtime;
using Expeditionary.View.Screens;

namespace Expeditionary.Runners.GameStates
{
    public class GalaxyState : IGameState
    {
        public event EventHandler<IGameStateContext>? GameStateChanged;

        private readonly ProgramConfig _config;
        private readonly GameModule _module;

        private IGameStateContext.GalaxyContext? _context;
        private GalaxyScreen? _screen;
        private GalaxyScreenController? _controller;

        public GalaxyState(ProgramConfig config, GameModule module)
        {
            _config = config;
            _module = module;
        }

        public IScreen Enter(object? context, ScreenFactory screenFactory)
        {
            _context = (IGameStateContext.GalaxyContext)context!;
            _screen = screenFactory.CreateGalaxy(_module, _context.MissionManager);
            _controller = (GalaxyScreenController)_screen.Controller;
            _controller.Launched += HandleLaunch;
            return _screen;
        }

        public void Exit()
        {
            _screen!.Dispose();
            _screen = null;

            _controller = null;
            _context = null;
        }

        private void HandleLaunch(object? sender, Mission e)
        {
            (var status, var task) =
                NewMatchLoader.Create(e, e.Content.Players.First().Player, _config.IsDebug, seed: 0);
            GameStateChanged?.Invoke(
                this,
                new IGameStateContext.LoadContext(
                    status, 
                    task.Map(
                        result => (IGameStateContext)new IGameStateContext.MatchContext(
                            result.Player, result.Match, result.Appearance, result.AiManager))));
        }
    }
}
