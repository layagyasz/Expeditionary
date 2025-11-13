using Cardamom.Ui.Controller;
using Expeditionary.Loader;
using Expeditionary.View.Screens;

namespace Expeditionary.Controller.Screens
{
    public class LoadScreenController : IController
    {
        public EventHandler<EventArgs>? Finished { get; set; }

        private readonly ILoaderTask _task;

        private LoadScreen? _screen;

        public LoadScreenController(ILoaderTask task)
        {
            _task = task;
        }

        public void Bind(object @object)
        {
            _screen = (LoadScreen)@object!;
            _screen.Updated += HandleUpdate;
        }

        public void Unbind()
        {
            _screen!.Updated -= HandleUpdate;
            _screen = null;
        }

        private void HandleUpdate(object? sender, EventArgs e)
        {
            if (_task.IsDone())
            {
                Finished?.Invoke(sender, e);
            }
        }
    }
}
