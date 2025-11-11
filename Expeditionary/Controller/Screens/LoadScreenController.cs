using Cardamom.Ui.Controller;
using Expeditionary.Loader;
using Expeditionary.View.Screens;

namespace Expeditionary.Controller.Screens
{
    public class LoadScreenController : IController
    {
        private readonly ILoaderTask _task;

        private LoadScreen? _screen;

        public LoadScreenController(ILoaderTask task)
        {
            _task = task;
        }

        public void Bind(object @object)
        {
            _screen = (LoadScreen?)@object;
        }

        public void Unbind()
        {
            _screen = null;
        }
    }
}
