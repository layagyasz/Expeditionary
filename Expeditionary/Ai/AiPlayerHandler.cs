using Cardamom;
using Cardamom.Logging;
using Expeditionary.Ai.Assignments;
using Expeditionary.Evaluation;
using Expeditionary.Model;
using Expeditionary.Model.Formations;
using Expeditionary.Model.Knowledge;

namespace Expeditionary.Ai
{
    public class AiPlayerHandler : IDisposable, IInitializable
    {
        private static readonly ILogger s_Logger = 
            new Logger(new ConsoleBackend(), LogLevel.Info).ForType(typeof(AiPlayerHandler));

        public Player Player { get; }

        private readonly Match _match;
        private readonly TileEvaluator _tileEvaluator;
        private readonly IPlayerKnowledge _knowledge;
        private readonly RootHandler _rootFormationHandler;

        public AiPlayerHandler(Player player, Match match, TileEvaluator tileEvaluator)
        {
            Player = player;
            _match = match;
            _tileEvaluator = tileEvaluator;
            _knowledge = match.GetKnowledge(Player);
            _rootFormationHandler = 
                new RootHandler(player, match.GetFormations(Player).Select(FormationHandler.Create));
        }

        public void AddFormation(Formation formation, IAssignment assignment)
        {
            var result = FormationHandler.Create(formation);
            _rootFormationHandler.Add(result);
            _rootFormationHandler.SetAssignment(assignment);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _match.FormationAdded -= HandleFormationAdded;
            s_Logger.With(Player.Id).Log("Disposed");
        }

        public void DoTurn()
        {
            s_Logger.With(Player.Id).Log($"started automated turn");
            _rootFormationHandler.DoTurn(_match, _knowledge, _tileEvaluator);
            foreach (var formation in _rootFormationHandler.Children)
            {
                DoFormationTurn(formation);
            }
            Thread.Sleep(1000);
            _match.Step();
            s_Logger.With(Player.Id).Log($"finished automated turn");
        }

        public void Initialize()
        {
            _match.FormationAdded += HandleFormationAdded;
            s_Logger.With(Player.Id).Log("Initialized");
        }

        public void Setup()
        {
            s_Logger.With(Player.Id).Log("Setup formations");
            _rootFormationHandler.DoTurn(_match, _knowledge, _tileEvaluator);
            foreach (var formationHandler in _rootFormationHandler.Children)
            {
                DoFormationTurn(formationHandler);
            }
        }

        private void DoFormationTurn(FormationHandler formation)
        {
            formation.DoTurn(_match, _knowledge, _tileEvaluator);
            foreach (var diad in formation.Diads)
            {
                diad.DoTurn(_match, _knowledge, _tileEvaluator);
            }
            foreach (var child in formation.Children)
            {
                DoFormationTurn(child);
            }
        }

        private void HandleFormationAdded(object? sender, Formation formation)
        {
            // Add formation in correct location in formation tree.
            s_Logger.With(Player.Id).AtWarning().Log("HandleFormationAdded not implemented");
        }
    }
}
