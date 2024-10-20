using Cardamom.Collections;
using Expeditionary.Model.Combat.Units;

namespace Expeditionary.Model.Orders
{
    public class OrderBuffer
    {
        public EventHandler<IOrder>? Added { get; set; }
        public EventHandler<IOrder>? Removed { get; set; }

        private readonly Match _match;
        private readonly MultiMap<Unit, IOrder> _orders = new();

        public OrderBuffer(Match match)
        {
            _match = match;
        }

        public bool Add(IOrder order)
        {
            if (order.Validate(_match))
            {
                _orders.Add(order.Unit, order);
                Added?.Invoke(this, order);
                return true;
            }
            return false;
        }

        public bool Remove(IOrder order)
        {
            return _orders.Remove(order.Unit, order);
        }

        public void Flush()
        {
            foreach (var order in _orders.Values.SelectMany(x => x))
            {
                order.Execute(_match);
            }
        }
    }
}
