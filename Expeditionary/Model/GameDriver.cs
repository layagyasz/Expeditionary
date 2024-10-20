using Expeditionary.Model.Orders;

namespace Expeditionary.Model
{
    public class GameDriver
    {
        public EventHandler<IOrder>? OrderAdded { get; set; }
        public EventHandler<IOrder>? OrderRemoved { get; set; }
        public EventHandler<EventArgs>? Stepped { get; set; }

        private readonly Match _match;
        private readonly List<Player> _players;
        private readonly OrderBuffer _orderBuffer;

        private int _activePlayer = -1;

        public GameDriver(Match match, IEnumerable<Player> players)
        {
            _match = match;
            _players = players.ToList();
            _orderBuffer = new(match);
            _orderBuffer.Added += HandleOrderAdded;
            _orderBuffer.Removed += HandleOrderRemoved;
        }

        public Match GetMatch()
        {
            return _match;
        }

        public bool AddOrder(IOrder order)
        {
            if (!ValidatePlayer(order.Unit.Player))
            {
                return false;
            }
            return _orderBuffer.Add(order);
        }

        public bool RemoveOrder(IOrder order)
        {
            if (!ValidatePlayer(order.Unit.Player))
            {
                return false;
            }
            return _orderBuffer.Remove(order);
        }

        public void Step()
        {
            _activePlayer++;
            _activePlayer %= _players.Count;
            _orderBuffer.Flush();
            Stepped?.Invoke(this, EventArgs.Empty);
        }

        private bool ValidatePlayer(Player player)
        {
            return player.Id == _activePlayer;
        }

        private void HandleOrderAdded(object? sender, IOrder order)
        {
            OrderAdded?.Invoke(this, order);
        }

        private void HandleOrderRemoved(object? sender, IOrder order)
        {
            OrderRemoved?.Invoke(this, order);
        }
    }
}
