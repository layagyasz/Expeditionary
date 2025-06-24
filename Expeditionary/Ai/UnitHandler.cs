using Cardamom.Collections;
using Cardamom.Logging;
using Expeditionary.Ai.Actions;
using Expeditionary.Ai.Assignments.Units;
using Expeditionary.Evaluation;
using Expeditionary.Model;
using Expeditionary.Model.Formations;
using Expeditionary.Model.Knowledge;
using Expeditionary.Model.Units;

namespace Expeditionary.Ai
{
    public class UnitHandler
    {
        protected readonly ILogger s_Logger =
            new Logger(new ConsoleBackend(), LogLevel.Info).ForType(typeof(UnitHandler));

        public Unit Unit { get; }
        public FormationRole Role { get; }
        public IUnitAssignment Assignment { get; private set; } = new NoUnitAssignment();

        public UnitHandler(Unit unit, FormationRole role)
        {
            Unit = unit;
            Role = role;
        }

        public void DoTurn(Match match, IPlayerKnowledge knowledge, TileEvaluator tileEvaluator)
        {
            if (!IsActive())
            {
                return;
            }
            var evaluator = tileEvaluator.GetEvaluatorFor(Unit, Assignment.Facing);
            var action =
                GenerateActions(match, knowledge)
                    .Select(Action => (Action, Value: Assignment.Evaluate(Unit, Action, evaluator, match)))
                    .ArgMax(x => x.Value);
            s_Logger.With(Unit.Id).Log($"action {action.Action} value {action.Value}");
            action.Action?.Do(match, Unit);
        }

        public void SetAssignment(IUnitAssignment assignment)
        {
            Assignment = assignment;
            s_Logger.With(Unit.Id).Log($"assigned {assignment}");
        }

        public bool IsActive()
        {
            return !Unit.IsDestroyed && Unit.Position != null;
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
