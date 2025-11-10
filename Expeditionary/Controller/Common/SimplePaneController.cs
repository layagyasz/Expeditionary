using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Expeditionary.View.Common;

namespace Expeditionary.Controller.Common
{
    public class SimplePaneController : IController
    {
        private IPane? _pane;

        public void Bind(object @object)
        {
            _pane = (IPane)@object;
            _pane.CloseButton.Controller.Clicked += HandleClose;
        }

        public void Unbind()
        {
            _pane!.CloseButton.Controller.Clicked -= HandleClose;
            _pane = null;
        }

        private void HandleClose(object? sender, MouseButtonClickEventArgs e)
        {
            _pane!.Visible = false;
        }
    }
}
