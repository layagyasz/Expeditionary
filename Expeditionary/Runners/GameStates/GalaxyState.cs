using Cardamom.Graphics;
using Expeditionary.Controller.Screens;
using Expeditionary.Model;
using Expeditionary.Model.Missions;
using Expeditionary.View.Screens;

namespace Expeditionary.Runners.GameStates
{
    public class GalaxyState : IGameState
    {
        public record class GalaxyContext(MissionManager MissionManager);

        public EventHandler<GameStateChangedEventArgs>? GameStateChanged { get; set; }

        public GameStateId Id => GameStateId.GalaxyOverview;

        private readonly GameModule _module;

        private GalaxyContext? _context;
        private GalaxyScreen? _screen;
        private GalaxyScreenController? _controller;

        public GalaxyState(GameModule module)
        {
            _module = module;
        }

        public IRenderable Enter(object? context, ScreenFactory screenFactory)
        {
            _context = (GalaxyContext)context!;
            _screen = screenFactory.CreateGalaxy(_module, _context.MissionManager);
            _controller = (GalaxyScreenController)_screen.Controller;
            return _screen;
        }

        public void Exit()
        {
            _screen!.Dispose();
            _screen = null;

            _controller = null;
            _context = null;
        }
    }
}
