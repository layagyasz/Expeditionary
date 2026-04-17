using Cardamom;
using Cardamom.Logging;

namespace Expeditionary.Model.Matches.Ai
{
    public class AiManager : IDisposable, IInitializable
    {
        private static readonly ILogger Logger =
            new Logger(new ConsoleBackend(), LogLevel.Info).ForType(typeof(AiManager));

        private readonly Match _match;
        private readonly Dictionary<MatchPlayer, AiPlayerHandler> _handlers = new();

        public AiManager(Match match)
        {
            _match = match;
        }

        public AiPlayerHandler Add(MatchPlayer player)
        {
            var handler = CreateHandler(player);
            _handlers.Add(player, handler);
            return handler;
        }

        public AiPlayerHandler CreateHandler(MatchPlayer player)
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

        public AiPlayerHandler? GetHandler(MatchPlayer player)
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
