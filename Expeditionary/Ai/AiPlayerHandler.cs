using Cardamom;
using Cardamom.Logging;
using Expeditionary.Ai.Assignments;
using Expeditionary.Model;
using Expeditionary.Model.Matches;

namespace Expeditionary.Ai
{
    public class AiPlayerHandler : IDisposable, IInitializable
    {
        private static readonly ILogger s_Logger = 
            new Logger(new ConsoleBackend(), LogLevel.Info).ForType(typeof(AiPlayerHandler));

        public Player Player { get; }

        private readonly Match _match;
        private readonly RootHandler _rootFormationHandler;

        public AiPlayerHandler(Player player, Match match)
        {
            Player = player;
            _match = match;
            _rootFormationHandler = 
                new RootHandler(player, match.GetFormations(Player).Select(FormationHandler.Create));
        }

        public void AddFormation(Formation formation, Formation? parent)
        {
            Precondition.Check(formation.Player == Player);
            var handler = FormationHandler.Create(formation);
            if (parent == null)
            {
                _rootFormationHandler.AddComponent(handler);
            }
            else
            {
                var parentHandler = 
                    _rootFormationHandler.GetAllComponents().First(handler => handler.Formation == parent);
                parentHandler.AddComponent(handler);
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            s_Logger.With(Player.Id).Log("Disposed");
        }

        public void DoTurn()
        {
            s_Logger.With(Player.Id).Log($"started automated turn");
            _rootFormationHandler.DoTurn(_match);
            _match.Step();
            s_Logger.With(Player.Id).Log($"finished automated turn");
        }

        public void Initialize()
        {
            s_Logger.With(Player.Id).Log("Initialized");
        }

        public void SetAssignment(IAssignment assignment)
        {
            _rootFormationHandler.SetAssignment(assignment);
        }

        public void Setup()
        {
            s_Logger.With(Player.Id).Log("Setup formations");
            _rootFormationHandler.Setup(_match);
        }
    }
}
