using Expeditionary.Ai.Actions;
using Expeditionary.Evaluation.Considerations;
using Expeditionary.Evaluation;
using Expeditionary.Model;
using Expeditionary.Model.Units;
using OpenTK.Mathematics;
using static Expeditionary.Evaluation.TileEvaluator;
using Expeditionary.Hexagons;
using Expeditionary.Model.Mapping;

namespace Expeditionary.Ai.Assignments.Units
{
    public record class PositionAssignment(MapDirection Facing, Vector3i Position) : IUnitAssignment
    {
        private static readonly float s_Reward = 20f;

        public void Place(UnitHandler unit, Match match)
        {
            match.Place(unit.Unit, Position);
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
                    tileEvaluator.GetThreatConsiderationFor(Disposition.Defensive, match);
                var baseline = TileConsiderations.Evaluate(consideration, unit.Position!.Value, match.GetMap());
                if (unit.Position != Position)
                {
                    var inDirection =
                        Geometry.GetCubicDistance(unit.Position!.Value, Position)
                        > Geometry.GetCubicDistance(moveAction.Path.Destination, Position) ? 1 : 0;
                    return unit.UnitQuantity.Points
                        * (TileConsiderations.Evaluate(
                            consideration, moveAction.Path.Destination, match.GetMap()) - baseline) +
                            s_Reward * (moveAction.Path.Cost / unit.Movement + inDirection);
                }
                else
                {
                    return unit.UnitQuantity.Points
                        * (TileConsiderations.Evaluate(
                            consideration, moveAction.Path.Destination, match.GetMap()) - baseline) - s_Reward;
                }
            }
            if (action is IdleAction)
            {
                return unit.Position == Position ? s_Reward : 0;
            }
            throw new ArgumentException($"Unsupported action {action}");
        }
    }
}
