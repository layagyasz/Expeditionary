using Cardamom;
using Cardamom.Logging;
using Expeditionary.Model;
using Expeditionary.Model.Matches;

namespace Expeditionary.Ai
{
    public class AiManager : IDisposable, IInitializable
    {
        private static readonly ILogger Logger = 
            new Logger(new ConsoleBackend(), LogLevel.Info).ForType(typeof(AiManager));

        private readonly Match _match;
        private readonly Dictionary<Player, AiPlayerHandler> _handlers;

        public AiManager(Match match, IEnumerable<Player> players)
        {
            _match = match;
            _handlers = players.ToDictionary(x => x, CreateHandler);
        }

        public AiPlayerHandler CreateHandler(Player player)
        {
            return new(player, _match);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _match.Stepped -= HandleStep;
            _match.FormationAdded -= HandleFormationAdded;
            foreach (var handler in _handlers.Values) 
            {
                handler.Dispose();
            }
            Logger.Log("Disposed");
        }

        public void Initialize()
        {
            _match.Stepped += HandleStep;
            _match.FormationAdded += HandleFormationAdded;
            foreach (var handler in _handlers.Values)
            {
                handler.Initialize();
            }
            Logger.Log("Initialized");
        }

        public AiPlayerHandler? GetHandler(Player player)
        {
            if (_handlers.TryGetValue(player, out var handler))
            {
                return handler;
            }
            return null;
        }

        private void HandleFormationAdded(object? sender, FormationAddedEventArgs e)
        {
            if (_handlers.TryGetValue(e.Formation.Player, out var handler))
            {
                handler.AddFormation(e.Formation, e.Parent);
            }
        }

        private void HandleStep(object? sender, EventArgs e)
        {
            if (_handlers.TryGetValue(_match.CurrentTurn.Player!, out var handler))
            {
                Task.Run(handler.DoTurn);
            }
        }
    }
}
