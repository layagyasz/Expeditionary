using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using Expeditionary.Model;
using Expeditionary.Model.Missions.Objectives;
using Expeditionary.View.Scenes.Matches.Overlays;

namespace Expeditionary.Controller.Scenes.Matches.Overlays
{
    public class ObjectiveComponentController : IController
    {
        private readonly Match _match;
        private readonly Player _player;
        private readonly IObjective _objective;

        private ObjectiveOverlay.ObjectiveComponent? _component;
        private OptionElementController<object>? _checkController;

        public ObjectiveComponentController(Match match, Player player, IObjective objective)
        {
            _match = match;
            _player = player;
            _objective = objective;
        }

        public void Bind(object @object)
        {
            _component = (ObjectiveOverlay.ObjectiveComponent)@object;
            _component.Refreshed += HandleRefresh;

            _checkController = (OptionElementController<object>)_component.Checkbox.Controller;
            _checkController.SetDisable(true);
        }

        public void Unbind()
        {
            _component!.Refreshed -= HandleRefresh;
            _component = null;

            _checkController = null;
        }

        private void HandleRefresh(object? sender, EventArgs e)
        {
            _checkController!.SetSelected(_objective.GetProgress(_player, _match).IsDone());
        }
    }
}
