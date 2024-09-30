using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using Cardamom.Window;
using OpenTK.Windowing.Common;

namespace Expeditionary.Controller
{
    public class SceneController : IElementController
    {
        public EventHandler<MouseButtonClickEventArgs>? Clicked { get; set; }
        public EventHandler<EventArgs>? Focused { get; set; }
        public EventHandler<EventArgs>? FocusLeft { get; set; }
        public EventHandler<MouseButtonDragEventArgs>? MouseDragged { get; set; }
        public EventHandler<EventArgs>? MouseEntered { get; set; }
        public EventHandler<EventArgs>? MouseLeft { get; set; }

        private readonly IElementController _cameraController;
        private readonly IController _mainController;

        public SceneController(IElementController cameraController, IController mainController)
        {
            _cameraController = cameraController;
            _mainController = mainController;
        }

        public void Bind(object @object)
        {
            _mainController.Bind(@object);
        }

        public void Unbind()
        {
            _mainController.Unbind();
        }

        public bool HandleKeyDown(KeyDownEventArgs e)
        {
            return _cameraController.HandleKeyDown(e);
        }

        public bool HandleTextEntered(TextEnteredEventArgs e)
        {
            return true;
        }

        public bool HandleMouseEntered()
        {
            return false;
        }

        public bool HandleMouseLeft()
        {
            return false;
        }

        public bool HandleMouseButtonClicked(MouseButtonClickEventArgs e)
        {
            return false;
        }

        public bool HandleMouseButtonDragged(MouseButtonDragEventArgs e)
        {
            return _cameraController.HandleMouseButtonDragged(e);
        }

        public virtual bool HandleMouseWheelScrolled(MouseWheelEventArgs e)
        {
            return _cameraController.HandleMouseWheelScrolled(e);
        }

        public bool HandleMouseLingered()
        {
            return false;
        }

        public bool HandleMouseLingerBroken()
        {
            return false;
        }

        public bool HandleFocusEntered()
        {
            Focused?.Invoke(this, EventArgs.Empty);
            return true;
        }

        public bool HandleFocusLeft()
        {
            return true;
        }
    }
}
