using Cardamom;
using Cardamom.Logging;
using Expeditionary.Evaluation;
using Expeditionary.Model;

namespace Expeditionary.Ai
{
    public class AiManager : IDisposable, IInitializable
    {
        private static readonly ILogger s_Logger = 
            new Logger(new ConsoleBackend(), LogLevel.Info).ForType(typeof(AiManager));

        private readonly Match _match;
        private readonly EvaluationCache _evaluationCache;
        private readonly Random _random;
        private readonly Dictionary<Player, AiPlayerHandler> _handlers;

        public AiManager(Match match, EvaluationCache evaluationCache, Random random, IEnumerable<Player> players)
        {
            _match = match;
            _evaluationCache = evaluationCache;
            _random = random;
            _handlers = players.ToDictionary(x => x, CreateHandler);
        }

        public AiPlayerHandler CreateHandler(Player player)
        {
            return new(player, _match, new(_evaluationCache, _random));
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _match.Stepped -= HandleStep;
            foreach (var handler in _handlers.Values) 
            {
                handler.Dispose();
            }
            s_Logger.Log("Disposed");
        }

        public void Initialize()
        {
            _match.Stepped += HandleStep;
            foreach (var handler in _handlers.Values)
            {
                handler.Initialize();
            }
            s_Logger.Log("Initialized");
        }

        public AiPlayerHandler? GetHandler(Player player)
        {
            if (_handlers.TryGetValue(player, out var handler))
            {
                return handler;
            }
            return null;
        }

        private void HandleStep(object? sender, EventArgs e)
        {
            if (_handlers.TryGetValue(_match.GetActivePlayer(), out var handler))
            {
                Task.Run(handler.DoTurn);
            }
        }
    }
}
