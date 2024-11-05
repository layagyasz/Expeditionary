using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using Cardamom.Window;
using Expeditionary.Model.Knowledge;
using Expeditionary.Model.Mapping;
using Expeditionary.View;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace Expeditionary.Controller
{
    public class FogOfWarLayerController : IElementController
    {
        public EventHandler<MouseButtonClickEventArgs>? Clicked { get; set; }
        public EventHandler<EventArgs>? Focused { get; set; }
        public EventHandler<EventArgs>? FocusLeft { get; set; }
        public EventHandler<MouseButtonDragEventArgs>? MouseDragged { get; set; }
        public EventHandler<EventArgs>? MouseEntered { get; set; }
        public EventHandler<EventArgs>? MouseLeft { get; set; }

        public EventHandler<AssetClickedEventArgs>? AssetClicked { get; set; }

        private FogOfWarLayer? _layer;

        public void Bind(object @object)
        {
            _layer = ((InteractiveModel)@object).GetModel() as FogOfWarLayer;
        }

        public void Unbind()
        {
            _layer = null;
        }

        public void SetKnowledge(PlayerKnowledge knowledge)
        {
            _layer!.SetAll(knowledge);
        }

        public void UpdateKnowledge(PlayerKnowledge knowledge, IEnumerable<Vector3i> delta)
        {
            _layer!.Set(knowledge, delta);
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
            
            return false;
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
    }
}
