using Cardamom.Graphics;
using Expeditionary.Ai;
using Expeditionary.Controller.Screens;
using Expeditionary.Model;
using Expeditionary.Model.Mapping.Appearance;
using Expeditionary.View.Screens;

namespace Expeditionary.Runners.GameStates
{
    public class MatchState : IGameState
    {
        public record class MatchContext(Player Player, Match Match, MapAppearance Appearance, AiManager AiManager);

        public EventHandler<GameStateChangedEventArgs>? GameStateChanged { get; set; }

        public GameStateId Id => GameStateId.Match;

        private readonly GameModule _module;

        private MatchContext? _context;
        private MatchScreen? _screen;
        private MatchController? _controller;

        public MatchState(GameModule module)
        {
            _module = module;
        }

        public IRenderable Enter(object? context, ScreenFactory screenFactory)
        {
            _context = (MatchContext)context!;
            _context.Match.Initialize();
            _context.AiManager.Initialize();
            _context.Match.Step();
            _screen = screenFactory.CreateMatch(_context.Match, _context.Appearance, _context.Player);
            _controller = (MatchController)_screen.Controller;
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
