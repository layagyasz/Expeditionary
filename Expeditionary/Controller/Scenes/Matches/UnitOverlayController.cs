using Cardamom.Ui.Controller;
using Expeditionary.View.Scenes.Matches;

namespace Expeditionary.Controller.Scenes.Matches
{
    public class UnitOverlayController : IController
    {
        public EventHandler<EventArgs>? OrderChanged { get; set; }

        private UnitOverlay? _overlay;
        private IFormFieldController<ButtonId>? _orderSelectController;

        public void Bind(object @object)
        {
            _overlay = @object as UnitOverlay;

            _orderSelectController = (IFormFieldController<ButtonId>)_overlay!.Orders.ComponentController;
            _orderSelectController.ValueChanged += HandleOrderChanged;
        }

        public void Unbind()
        {
            _overlay = null;

            _orderSelectController!.ValueChanged -= HandleOrderChanged;
            _orderSelectController = null;
        }

        public ButtonId GetOrder()
        {
            return _orderSelectController!.GetValue();
        }

        public void SetOrder(ButtonId order)
        {
            _orderSelectController!.Set(order);
        }

        private void HandleOrderChanged(object? sender, EventArgs e)
        {
            OrderChanged?.Invoke(this, e);
        }
    }
}
