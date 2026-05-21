using Expeditionary.Controller.Screens;
using Expeditionary.Model;
using Expeditionary.Model.Instances;
using Expeditionary.Model.Instances.Events;
using Expeditionary.Model.Matches.Reporting;
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
            ApplyResults(_context!);
            GameStateChanged?.Invoke(this, GetNextContext(_context!));
        }

        private static IGameStateContext GetNextContext(IGameStateContext.MatchContext currentContext)
        {
            return currentContext.Instance == null 
                ? new IGameStateContext.MainMenuContext() 
                : new IGameStateContext.GalaxyContext(currentContext.Instance);
        }

        private static void ApplyResults(IGameStateContext.MatchContext context)
        {
            if (context.Instance == null || context.StageKey == null)
            {
                return;
            }
            var report = MatchReport.Generate(context.Match);
            var instanceReport = new InstanceMatchReport(context.StageKey!.Value, context.Player, report);
            context.Instance.Apply(new AddMatchReportEvent(instanceReport));
            foreach (var @event in instanceReport.GetEvents())
            {
                context.Instance.Apply(@event);
            }
        }
    }
}
