using Cardamom.Ui.Controller;
using Expeditionary.Model;
using Expeditionary.Model.Units;
using Expeditionary.View.Scenes.Matches;
using Expeditionary.View.Scenes.Matches.Overlays;

namespace Expeditionary.Controller.Scenes.Matches.Overlays
{
    public class UnitOverlayController : IController
    {
        public EventHandler<EventArgs>? OrderChanged { get; set; }

        private Match? _match;

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

        public void SetMatch(Match match)
        {
            _match = match;
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
                _overlay!.Title.SetText(unit.Name);
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

        private IEnumerable<OrderValue> GetPossibleOrders(Unit unit)
        {
            if (unit.Actions == 0 || !unit.IsActive())
            {
                yield break;
            }
            foreach (var weapon in unit.Type.Weapons)
            {
                foreach (var mode in weapon.Weapon.Modes)
                {
                    yield return new(FormatAttackName(weapon, mode), OrderId.Attack, new object[] { weapon, mode });
                }
            }
            if (unit.Type.Speed >= 1)
            {
                yield return new("Move", OrderId.Move, Array.Empty<object>());
            }
            foreach (var passenger in _match!.GetAssetsAt(unit.Position!.Value)
                .Where(x => OrderChecker.CanLoad(unit, x)))
            {
                yield return new($"Load {passenger.Name}", OrderId.Load, new object[] { passenger });
            }
            if (unit.Passenger != null)
            {
                yield return new($"Unload {unit.Passenger.Name}", OrderId.Unload, Array.Empty<object>());
            }
        }

        private static string FormatAttackName(UnitWeaponUsage weapon, UnitWeapon.Mode mode)
        {
            return $"{weapon.Weapon.Name}({mode.Condition})";
        }
    }
}
