using Cardamom;
using Cardamom.Logging;
using Expeditionary.Model;

namespace Expeditionary.Ai
{
    public class AiManager : IDisposable, IInitializable
    {
        private static readonly ILogger s_Logger = 
            new Logger(new ConsoleBackend(), LogLevel.Info).ForType(typeof(AiManager));

        private readonly Match _match;
        private readonly Dictionary<Player, AiPlayer> _players;

        private AiManager(Match match, Dictionary<Player, AiPlayer> players)
        {
            _match = match;
            _players = players;
        }

        public static AiManager Create(Match match, IEnumerable<Player> players)
        {
            return new(match, players.ToDictionary(x => x, x => new AiPlayer(x, match)));
        }

        public void Dispose()
        {
            _match.Stepped -= HandleStep;
            s_Logger.Log("Disposed");
        }

        public void Initialize()
        {
            _match.Stepped += HandleStep;
            s_Logger.Log("Initialized");
        }

        private void HandleStep(object? sender, EventArgs e)
        {
            if (_players.TryGetValue(_match.GetActivePlayer(), out var handler))
            {
                Task.Run(handler.DoTurn);
            }
        }
    }
}
