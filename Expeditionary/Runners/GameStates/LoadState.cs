using Expeditionary.Controller.Screens;
using Expeditionary.Loader;
using Expeditionary.View.Screens;

namespace Expeditionary.Runners.GameStates
{
    public class LoadState : IGameState
    {
        private static readonly int WaitMillis = 200;

        public event EventHandler<IGameStateContext>? GameStateChanged;

        private readonly ThreadedLoader _loader;

        private IGameStateContext.LoadContext? _context;
        private LoadScreen? _screen;
        private LoadScreenController? _controller;

        public LoadState(ThreadedLoader loader)
        {
            _loader = loader;
        }

        public IScreen Enter(object? context, ScreenFactory screenFactory)
        {
            _context = (IGameStateContext.LoadContext)context!;
            var task = _context.Task.Map(Wait);
            _screen = screenFactory.CreateLoad(task, _context.Status);
            _controller = (LoadScreenController)_screen.Controller;
            _controller.Finished += HandleFinished;
            _loader.Load(task);
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
            GameStateChanged?.Invoke(this, _context!.Task.GetPromise().Get());
        }

        private static T Wait<T>(T value)
        {
            Thread.Sleep(WaitMillis);
            return value;
        }
    }
}
