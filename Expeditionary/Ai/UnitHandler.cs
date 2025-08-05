using Cardamom.Collections;
using Cardamom.Logging;
using Expeditionary.Ai.Actions;
using Expeditionary.Ai.Assignments;
using Expeditionary.Model;
using Expeditionary.Model.Formations;
using Expeditionary.Model.Knowledge;
using Expeditionary.Model.Units;

namespace Expeditionary.Ai
{
    public class UnitHandler : IAiHandler
    {
        protected readonly ILogger s_Logger =
            new Logger(new ConsoleBackend(), LogLevel.Info).ForType(typeof(UnitHandler));

        public FormationRole Role { get; }
        public Unit Unit { get; }
        public IAssignment Assignment { get; private set; } = new NoAssignment(default);
        public IEnumerable<FormationHandler> Children => Enumerable.Empty<FormationHandler>();
        public IEnumerable<DiadHandler> Diads => Enumerable.Empty<DiadHandler>();
        public string Id => $"unit-{Unit.Id}";
        public int Echelon => 1;

        public UnitHandler(FormationRole role, Unit unit)
        {
            Unit = unit;
            Role = role;
        }

        public void Add(FormationHandler handler)
        {
            throw new NotImplementedException();
        }

        public void DoTurn(Match match)
        {
            if (!Unit.IsActive())
            {
                return;
            }
            var evaluator = match.GetEvaluatorFor(Unit, Assignment.Facing);
            var action =
                Assignment.EvaluateActions(GenerateActions(match, match.GetKnowledge(Unit.Player)), Unit, match)
                    .ArgMax(x => x.Item2);
            s_Logger.With(Unit.Id).Log($"action {action.Item1} value {action.Item2}");
            if (action.Item1?.Do(match, Unit) ?? false)
            {
                Assignment.NotifyAction(Unit, action.Item1!, match);
            }
            else
            {
                s_Logger.With(Unit.Id).AtError().Log($"action {action.Item1} failed");
            }
        }

        public Movement.Hindrance GetMaxHindrance()
        {
            return Unit.Type.Movement.GetMaxHindrance();
        }

        public void Setup(Match match)
        {
            match.Place(Unit, Assignment.SelectHex(match.GetMap()));
        }

        public void SetAssignment(IAssignment assignment)
        {
            Assignment = assignment;
            s_Logger.With(Unit.Id).Log($"assigned {assignment}");
        }

        private IEnumerable<IUnitAction> GenerateActions(Match match, IPlayerKnowledge knowledge)
        {
            if (Unit.Actions > 0)
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
}
