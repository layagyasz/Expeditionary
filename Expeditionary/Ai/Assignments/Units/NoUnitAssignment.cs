using Expeditionary.Ai.Actions;
using Expeditionary.Evaluation;
using Expeditionary.Evaluation.Considerations;
using Expeditionary.Model;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Units;
using static Expeditionary.Evaluation.TileEvaluator;

namespace Expeditionary.Ai.Assignments.Units
{
    public class NoUnitAssignment : IUnitAssignment
    {
        public MapDirection Facing => MapDirection.All;

        public void Place(UnitHandler unit, Match match)
        {
            throw new NotImplementedException();
        }

        public float Evaluate(Unit unit, IUnitAction action, UnitTileEvaluator tileEvaluator, Match match)
        {
            if (action is AttackAction attackAction)
            {
                return AttackEvaluation.Evaluate(
                    unit, attackAction.Attack, attackAction.Mode, attackAction.Target, match.GetMap());
            }
            if (action is MoveAction moveAction)
            {
                var consideration = 
                    tileEvaluator.GetThreatConsiderationFor(DefaultDispositionMapper.Map(unit.Type), match);
                var baseline = TileConsiderations.Evaluate(consideration, unit.Position!.Value, match.GetMap());
                return unit.UnitQuantity.Points
                    * (TileConsiderations.Evaluate(
                        consideration, moveAction.Path.Destination, match.GetMap()) - baseline);
            }
            if (action is IdleAction)
            {
                return 0;
            }
            throw new ArgumentException($"Unsupported action {action}");
        }
    }
}
