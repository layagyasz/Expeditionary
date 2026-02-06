using Cardamom.Graphics;
using Expeditionary.Controller.Screens;
using Expeditionary.Loader;
using Expeditionary.View.Screens;

namespace Expeditionary.Runners.GameStates
{
    public class LoadState : IGameState
    {
        public record class LoadContext(GameStateId NextState, LoaderStatus Status, LoaderTaskNode<object?> Task);

        private static readonly int i_Wait = 200;

        public EventHandler<GameStateChangedEventArgs>? GameStateChanged { get; set; }

        public GameStateId Id => GameStateId.Load;

        private readonly ThreadedLoader _loader;

        private LoadContext? _context;
        private LoadScreen? _screen;
        private LoadScreenController? _controller;

        public LoadState(ThreadedLoader loader)
        {
            _loader = loader;
        }

        public IScreen Enter(object? context, ScreenFactory screenFactory)
        {
            _context = (LoadContext)context!;
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
            GameStateChanged?.Invoke(this, new(_context!.NextState, _context!.Task.GetPromise().Get()));
        }

        private static T Wait<T>(T value)
        {
            Thread.Sleep(i_Wait);
            return value;
        }
    }
}
