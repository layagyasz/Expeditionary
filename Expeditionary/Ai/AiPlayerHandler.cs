using Cardamom;
using Cardamom.Collections;
using Cardamom.Logging;
using Expeditionary.Ai.Assignments.Formations;
using Expeditionary.Evaluation;
using Expeditionary.Model;
using Expeditionary.Model.Formations;
using Expeditionary.Model.Knowledge;
using Expeditionary.Model.Orders;

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
        private readonly RootFormationHandler _rootFormationHandler;

        public AiPlayerHandler(Player player, Match match, TileEvaluator tileEvaluator)
        {
            Player = player;
            _match = match;
            _tileEvaluator = tileEvaluator;
            _knowledge = match.GetKnowledge(Player);
            _rootFormationHandler = 
                new RootFormationHandler(player, match.GetFormations(Player).Select(SimpleFormationHandler.Create));
        }

        public void AddFormation(Formation formation, IFormationAssignment assignment)
        {
            var result = SimpleFormationHandler.Create(formation);
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
            _rootFormationHandler.Reevaluate(_match, _tileEvaluator);
            foreach (var formation in _rootFormationHandler.GetAllFormationHandlers())
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
            _rootFormationHandler.Reevaluate(_match, _tileEvaluator);
            foreach (var formationHandler in _rootFormationHandler.GetAllFormationHandlers())
            {
                formationHandler.Reevaluate(_match, _tileEvaluator);
            }
            s_Logger.With(Player.Id).Log("Setup units");
            foreach (var unit in _rootFormationHandler.GetAllUnitHandlers())
            {
                unit.Assignment.Place(unit, _match);
            }
        }

        private void DoFormationTurn(SimpleFormationHandler formation)
        {
            formation.Reevaluate(_match, _tileEvaluator);
            foreach (var unit in formation.GetUnitHandlers())
            {
                unit.DoTurn(_match, _knowledge, _tileEvaluator);
            }
        }

        private void HandleFormationAdded(object? sender, Formation formation)
        {
            // Add formation in correct location in formation tree.
            s_Logger.With(Player.Id).AtWarning().Log("HandleFormationAdded not implemented");
        }
    }
}
