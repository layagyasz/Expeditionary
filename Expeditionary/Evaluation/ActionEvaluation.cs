using Expeditionary.Evaluation.Considerations;
using Expeditionary.Evaluation.TileEvaluators;
using Expeditionary.Model;
using Expeditionary.Model.Combat;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Units;

namespace Expeditionary.Evaluation
{
    public static class ActionEvaluation
    {
        private static readonly float s_PathReward = 20f;

        public static float EvaluateAttack(
            Unit attacker, UnitWeaponUsage attack, UnitWeapon.Mode mode, Unit defender, Map map)
        {
            var preview =
                CombatCalculator.GetPreview(
                    attacker, attacker.Position!.Value, attack, mode, defender, defender.Position!.Value, map);
            return defender.Type.Points * Math.Min(1, preview.Result / defender.Number);
        }

        public static float EvaluateMove(
            Unit unit, Pathing.PathOption path, Match match, UnitTileEvaluator tileEvaluator)
        {
            var consideration =
                    tileEvaluator.GetThreatConsiderationFor(Disposition.Defensive, match);
            var baseline = TileConsiderations.Evaluate(consideration, unit.Position!.Value, match.GetMap());
            return unit.Value.Points 
                * (TileConsiderations.Evaluate(consideration, path.Destination, match.GetMap()) - baseline);
        }

        public static float EvaluationMovePathBonus(Unit unit, Pathing.PathOption path, Pathing.Path goal)
        {
            if (goal.Steps.Contains(path.Destination))
            {
                return s_PathReward * (path.Cost / unit.Type.Speed);
            }
            return 0;
        }

        public static float EvaluateUnload(Unit unit, Match match, UnitTileEvaluator tileEvaluator)
        {
            if (unit.Passenger == null)
            {
                return 0;
            }
            if (unit.Passenger is Unit passengerUnit)
            {
                var baselineConsideration = TileConsiderations.Evaluate(
                    tileEvaluator.GetThreatConsiderationFor(Disposition.Defensive, match),
                    unit.Position!.Value,
                    match.GetMap());
                var baseline = (unit.Value.Points + unit.Passenger.Value.Points) * baselineConsideration;
                var passenger = 
                    TileConsiderations.Evaluate(
                        tileEvaluator
                            .ForPassenger(passengerUnit)
                            .GetThreatConsiderationFor(Disposition.Defensive, match),
                        unit.Position!.Value, 
                        match.GetMap());
                return passengerUnit.Value.Points * passenger + unit.Value.Points * baselineConsideration - baseline;
            }
            else
            {
                return 0;
            }
        }
    }
}
