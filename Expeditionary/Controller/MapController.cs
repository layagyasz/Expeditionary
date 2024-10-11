using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using Cardamom.Window;
using Expeditionary.Hexagons;
using Expeditionary.View.Mapping;
using OpenTK.Windowing.Common;

namespace Expeditionary.Controller
{
    public class MapController : IElementController
    {
        public EventHandler<MouseButtonClickEventArgs>? Clicked { get; set; }
        public EventHandler<EventArgs>? Focused { get; set; }
        public EventHandler<EventArgs>? FocusLeft { get; set; }
        public EventHandler<MouseButtonDragEventArgs>? MouseDragged { get; set; }
        public EventHandler<EventArgs>? MouseEntered { get; set; }
        public EventHandler<EventArgs>? MouseLeft { get; set; }

        public EventHandler<HexClickedEventArgs>? HexClicked { get; set; }

        private MapView? _map;

        public void Bind(object @object)
        {
            _map = ((InteractiveModel)@object).GetModel() as MapView;
        }

        public void Unbind()
        {
            _map = null;
        }

        public bool HandleKeyDown(KeyDownEventArgs e)
        {
            return false;
        }

        public bool HandleTextEntered(TextEnteredEventArgs e)
        {
            return false;
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
            HexClicked?.Invoke(this, new(Geometry.SnapToHex(Cubic.Cartesian.Instance.Wrap(e.Position.Xz)), e));
            return true;
        }

        public bool HandleMouseButtonDragged(MouseButtonDragEventArgs e)
        {
            return false;
        }

        public virtual bool HandleMouseWheelScrolled(MouseWheelEventArgs e)
        {
            return false;
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
            return false;
        }

        public bool HandleFocusLeft()
        {
            return false;
        }

        public void UpdateGridAlpha(float distance)
        {
            _map!.SetGridAlpha(GetGridAlpha(distance));
        }

        private static float GetGridAlpha(float distance)
        {
            if (distance < 10)
            {
                return 1;
            }
            if (distance > 70)
            {
                return 0;
            }
            return .017f * (60 - distance);
        }
    }
}
