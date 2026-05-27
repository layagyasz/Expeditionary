using Expeditionary.Controller.Screens;
using Expeditionary.Model;
using Expeditionary.View.Screens;

namespace Expeditionary.Runners.GameStates
{
    public class MatchSummaryState : IGameState
    {
        public event EventHandler<IGameStateContext>? GameStateChanged;

        private readonly GameModule _module;

        private IGameStateContext.MatchSummaryContext? _context;
        private MatchSummaryScreen? _screen;
        private MatchSummaryScreenController? _controller;

        public MatchSummaryState(GameModule module)
        {
            _module = module;
        }
        public IScreen Enter(object? context, ScreenFactory screenFactory)
        {
            _context = (IGameStateContext.MatchSummaryContext)context!;
            _screen = screenFactory.CreateMatchSummary(_context.Player, _context.Report);
            _controller = (MatchSummaryScreenController)_screen.Controller;
            _controller.Continued += HandleContinue;
            return _screen;
        }

        public void Exit()
        {
            _screen!.Dispose();
            _screen = null;

            _controller!.Continued -= HandleContinue;
            _controller = null;
        }

        private void HandleContinue(object? sender, EventArgs e)
        {
            GameStateChanged?.Invoke(this, GetNextContext(_context!));
        }

        private static IGameStateContext GetNextContext(IGameStateContext.MatchSummaryContext currentContext)
        {
            return currentContext.Instance == null
                ? new IGameStateContext.MainMenuContext()
                : new IGameStateContext.GalaxyContext(currentContext.Instance);
        }
    }
}
