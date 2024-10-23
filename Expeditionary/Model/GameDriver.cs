using Cardamom.Logging;
using Expeditionary.Model.Orders;

namespace Expeditionary.Model
{
    public class GameDriver
    {
        private static readonly ILogger s_Logger = new Logger(new ConsoleBackend(), LogLevel.Info);

        public EventHandler<IOrder>? OrderAdded { get; set; }
        public EventHandler<IOrder>? OrderRemoved { get; set; }
        public EventHandler<EventArgs>? Stepped { get; set; }

        private readonly Match _match;
        private readonly List<Player> _players;
        private readonly Random _random;

        private int _activePlayer = -1;

        public GameDriver(Match match, IEnumerable<Player> players, Random random)
        {
            _match = match;
            _players = players.ToList();
            _random = random;
        }

        public Match GetMatch()
        {
            return _match;
        }

        public bool DoOrder(IOrder order)
        {
            if (!ValidatePlayer(order.Unit.Player))
            {
                s_Logger.Log($"{order} failed player validation");
                return false;
            }
            if (!order.Validate(_match))
            {
                s_Logger.Log($"{order} failed match validation");
                return false;
            }
            s_Logger.Log($"{order} executed");
            order.Execute(_match, _random);
            return true;
        }

        public void Step()
        {
            _activePlayer++;
            if (_activePlayer >= _players.Count)
            {
                _activePlayer = 0;
                _match.Reset();
            }
            Stepped?.Invoke(this, EventArgs.Empty);
        }

        private bool ValidatePlayer(Player player)
        {
            return player.Id == _activePlayer;
        }
    }
}
