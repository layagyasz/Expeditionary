using Cardamom.Ui.Controller;
using Expeditionary.Controller.Common;
using Expeditionary.Model.Factions;
using Expeditionary.View.Screens;

namespace Expeditionary.Controller.Screens
{
    public class InstanceSetupScreenController : IController
    {
        public EventHandler<GameInstanceParameters>? Submitted { get; set; }

        private InstanceSetupScreen? _screen;
        private ButtonMenuController? _factionSelectController;

        public void Bind(object @object)
        {
            _screen = (InstanceSetupScreen)@object;

            _factionSelectController = (ButtonMenuController)_screen.FactionSelect!.ComponentController;
            _factionSelectController.OptionClicked += HandleMenuClicked;
        }

        public void Unbind()
        {
            _screen = null;

            _factionSelectController!.OptionClicked -= HandleMenuClicked;
            _factionSelectController = null;
        }

        private void HandleMenuClicked(object? sender, object item)
        {
            Submitted?.Invoke(this, new((Faction)item, Seed: 0));
        }
    }
}
