using Cardamom.Ui.Controller;
using Expeditionary.Controller.Common;
using Expeditionary.View.Screens;

namespace Expeditionary.Controller.Screens
{
    public class MainMenuScreenController : IController
    {
        public EventHandler<object>? MenuClicked { get; set; }

        private MainMenuScreen? _screen;
        private ButtonMenuController? _menuController;

        public void Bind(object @object)
        {
            _screen = (MainMenuScreen)@object;

            _menuController = (ButtonMenuController)_screen.Menu!.ComponentController;
            _menuController.OptionClicked += HandleMenuClicked;
        }

        public void Unbind()
        {
            _screen = null;

            _menuController!.OptionClicked -= HandleMenuClicked;
            _menuController = null;
        }

        private void HandleMenuClicked(object? sender, object item)
        {
            MenuClicked?.Invoke(this, item);
        }
    }
}
