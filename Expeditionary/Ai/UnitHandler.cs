using Cardamom.Collections;
using Cardamom.Logging;
using Expeditionary.Ai.Actions;
using Expeditionary.Ai.Assignments;
using Expeditionary.Evaluation;
using Expeditionary.Model;
using Expeditionary.Model.Formations;
using Expeditionary.Model.Knowledge;
using Expeditionary.Model.Units;

namespace Expeditionary.Ai
{
    public class UnitHandler : IFormationHandler
    {
        protected readonly ILogger s_Logger =
            new Logger(new ConsoleBackend(), LogLevel.Info).ForType(typeof(UnitHandler));

        public Unit Unit { get; }
        public FormationRole Role { get; }
        public IAssignment Assignment { get; private set; } = new NoAssignment();
        public IEnumerable<SimpleFormationHandler> Children => Enumerable.Empty<SimpleFormationHandler>();
        public string Id => $"unit-{Unit.Id}";
        public int Echelon => 1;

        public UnitHandler(Unit unit, FormationRole role)
        {
            Unit = unit;
            Role = role;
        }

        public void Add(SimpleFormationHandler handler)
        {
            throw new NotImplementedException();
        }

        public void DoTurn(Match match, IPlayerKnowledge knowledge, TileEvaluator tileEvaluator)
        {
            if (!Unit.IsActive())
            {
                return;
            }
            var evaluator = tileEvaluator.GetEvaluatorFor(Unit, Assignment.Facing);
            var action =
                GenerateActions(match, knowledge)
                    .Select(Action => (Action, Value: Assignment.EvaluateAction(Unit, Action, evaluator, match)))
                    .ArgMax(x => x.Value);
            s_Logger.With(Unit.Id).Log($"action {action.Action} value {action.Value}");
            action.Action?.Do(match, Unit);
        }

        public IEnumerable<SimpleFormationHandler> GetAllFormationHandlers()
        {
            return Enumerable.Empty<SimpleFormationHandler>();
        }

        public IEnumerable<UnitHandler> GetAllUnitHandlers()
        {
            return Enumerable.Empty<UnitHandler>();
        }

        public IEnumerable<UnitHandler> GetUnitHandlers()
        {
            return Enumerable.Empty<UnitHandler>();
        }

        public void Reevaluate(Match match, TileEvaluator tileEvaluator) { }

        public void SetAssignment(IAssignment assignment)
        {
            Assignment = assignment;
            s_Logger.With(Unit.Id).Log($"assigned {assignment}");
        }

        private IEnumerable<IUnitAction> GenerateActions(Match match, IPlayerKnowledge knowledge)
        {
            foreach (var attack in AttackAction.GenerateValidAttacks(match, knowledge, Unit))
            {
                yield return attack;
            }
            foreach (var move in MoveAction.GenerateValidMoves(match, Unit))
            {
                yield return move;
            }
            yield return new IdleAction();
        }
    }
}
