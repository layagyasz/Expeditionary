using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Expeditionary.View.Screens;

namespace Expeditionary.Controller.Screens
{
    public class MatchSummaryScreenController : IController
    {
        public event EventHandler<EventArgs>? Continued;

        private MatchSummaryScreen? _screen;

        public void Bind(object @object)
        {
            _screen = (MatchSummaryScreen)@object;
            _screen.ContinueButton.Controller.Clicked += HandleContinue;
        }

        public void Unbind()
        {
            _screen!.ContinueButton.Controller.Clicked -= HandleContinue;
            _screen = null;
        }

        private void HandleContinue(object? @object, MouseButtonClickEventArgs e)
        {
            Continued?.Invoke(this, EventArgs.Empty);
        }
    }
}
