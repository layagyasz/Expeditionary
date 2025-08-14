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

        public AiHandlerStatus DoTurn(Match match)
        {
            if (!Unit.IsActive())
            {
                return AiHandlerStatus.Inactive;
            }
            var evaluator = match.GetEvaluatorFor(Unit, Assignment.Facing);
            var actions = GenerateActions(match, match.GetKnowledge(Unit.Player)).ToArray();
            var evaluations = new float[actions.Length];
            CombineEvaluations(evaluations, UnitActionEvaluations.EvaluateDefault(actions, Unit, match, evaluator));
            CombineEvaluations(evaluations, Assignment.EvaluateActions(actions, Unit, match));
            var action = actions[Enumerable.Range(0, actions.Length).MaxBy(x => evaluations[x])];
            if (action.Do(match, Unit))
            {
                return Assignment.NotifyAction(Unit, action, match) 
                    ? AiHandlerStatus.Done : AiHandlerStatus.InProgress;
            }
            else
            {
                return AiHandlerStatus.Done;
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
            if (Unit.Actions > 0 && !Unit.IsPassenger)
            {
                foreach (var attack in AttackAction.GenerateValidAttacks(match, knowledge, Unit))
                {
                    yield return attack;
                }
                foreach (var load in LoadAction.GenerateValidLoads(match, Unit))
                {
                    yield return load;
                }
                foreach (var move in MoveAction.GenerateValidMoves(match, Unit))
                {
                    yield return move;
                }
                foreach (var unload in UnloadAction.GenerateValidUnloads(Unit))
                {
                    yield return unload;
                }
            }
            yield return new IdleAction();
        }

        private static void CombineEvaluations(float[] currentEvaluations, IEnumerable<float> newEvaluations)
        {
            int i = 0;
            foreach (var evaluation in newEvaluations)
            {
                currentEvaluations[i++] += evaluation;
            }
        }
    }
}
