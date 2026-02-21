using Cardamom.Ui.Controller;
using Expeditionary.Model.Missions.Objectives;
using Expeditionary.View;
using Expeditionary.View.Scenes.Matches.Overlays;

namespace Expeditionary.Controller.Scenes.Matches.Overlays
{
    public class FinishedOverlayController : IController
    {
        private readonly Localization _localization;

        private FinishedOverlay? _overlay;

        public FinishedOverlayController(Localization localization)
        {
            _localization = localization;
        }

        public void Bind(object @object)
        {
            _overlay = (FinishedOverlay)@object;
            _overlay.Visible = false;
        }
        
        public void Unbind()
        {
            _overlay = null;
        }

        public void Activate(ObjectiveStatus status)
        {
            _overlay!.Visible = true;
            // Localize
            _overlay.Text.SetText(status.ToString());
        }
    }
}
