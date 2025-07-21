using Cardamom.Ui.Controller;
using Expeditionary.Model.Units;
using Expeditionary.View.Scenes.Matches;

namespace Expeditionary.Controller.Scenes.Matches
{
    public class UnitOverlayController : IController
    {
        public EventHandler<EventArgs>? OrderChanged { get; set; }

        private UnitOverlay? _overlay;
        private IFormFieldController<OrderValue>? _orderSelectController;

        public void Bind(object @object)
        {
            _overlay = @object as UnitOverlay;

            _orderSelectController = (IFormFieldController<OrderValue>)_overlay!.Orders.ComponentController;
            _orderSelectController.ValueChanged += HandleOrderChanged;
        }

        public void Unbind()
        {
            _overlay = null;

            _orderSelectController!.ValueChanged -= HandleOrderChanged;
            _orderSelectController = null;
        }

        public OrderValue? GetOrder()
        {
            return _orderSelectController!.GetValue();
        }

        public void SetUnit(Unit? unit)
        {
            _overlay!.Orders.Clear(true);
            if (unit == null)
            {
                _overlay!.Title.SetText(string.Empty);
            }
            else
            {
                _overlay!.Title.SetText($"{unit.Type.Name} (#{unit.Id})");
                foreach (var order in GetPossibleOrders(unit))
                {
                    _overlay.AddOrder(order);
                }
            }
        }

        private void HandleOrderChanged(object? sender, EventArgs e)
        {
            OrderChanged?.Invoke(this, e);
        }

        private static IEnumerable<OrderValue> GetPossibleOrders(Unit unit)
        {
            foreach (var weapon in unit.Type.Weapons)
            {
                foreach (var mode in weapon.Weapon.Modes)
                {
                    yield return new(FormatAttackName(weapon, mode), ButtonId.Attack, new object[] { weapon, mode });
                }
            }
            if (unit.Type.Speed >= 1)
            {
                yield return new("Move", ButtonId.Move, Array.Empty<object>());
            }
        }

        private static string FormatAttackName(UnitWeaponUsage weapon, UnitWeapon.Mode mode)
        {
            return $"{weapon.Weapon.Name}({mode.Condition}";
        }
    }
}
