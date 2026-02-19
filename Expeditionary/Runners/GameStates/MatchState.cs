using Expeditionary.Controller.Screens;
using Expeditionary.Model;
using Expeditionary.View.Screens;

namespace Expeditionary.Runners.GameStates
{
    public class MatchState : IGameState
    {
        public event EventHandler<IGameStateContext>? GameStateChanged;

        private readonly GameModule _module;

        private IGameStateContext.MatchContext? _context;
        private MatchScreen? _screen;
        private MatchController? _controller;

        public MatchState(GameModule module)
        {
            _module = module;
        }

        public IScreen Enter(object? context, ScreenFactory screenFactory)
        {
            _context = (IGameStateContext.MatchContext)context!;
            _context.Match.Initialize();
            _context.AiManager.Initialize();
            _context.Match.Step();
            _screen = screenFactory.CreateMatch(_context.Match, _context.Appearance, _context.Player);
            _controller = (MatchController)_screen.Controller;
            _controller.Finished += HandleFinished;
            return _screen;
        }

        public void Exit()
        {
            _screen!.Dispose();
            _screen = null;

            _controller!.Finished -= HandleFinished;
            _controller = null;
            _context = null;
        }

        private void HandleFinished(object? sender, EventArgs e) 
        {
            GameStateChanged?.Invoke(this, new IGameStateContext.MainMenuContext());
        }
    }
}
