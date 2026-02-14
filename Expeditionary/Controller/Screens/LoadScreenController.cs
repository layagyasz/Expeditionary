using Cardamom.Ui.Controller;
using Expeditionary.Events;
using Expeditionary.Loader;

namespace Expeditionary.Controller.Screens
{
    public class LoadScreenController : IController, IEventDispatchable
    {
        public EventHandler<EventArgs>? Finished { get; set; }

        private readonly ILoaderTask _task;

        public LoadScreenController(ILoaderTask task)
        {
            _task = task;
        }

        public void Bind(object @object) { }

        public void Unbind() { }

        public void DispatchEvents(long delta)
        {
            if (_task.IsDone())
            {
                Finished?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
