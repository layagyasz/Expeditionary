using Expeditionary.Evaluation.Considerations;
using Expeditionary.Model;
using Expeditionary.Model.Combat;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Units;
using static Expeditionary.Evaluation.TileEvaluator;

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

        public static float EvaluateFreeMove(
            Unit unit, Pathing.PathOption path, UnitTileEvaluator tileEvaluator, Match match)
        {
            var consideration =
                    tileEvaluator.GetThreatConsiderationFor(Disposition.Defensive, match);
            var baseline = TileConsiderations.Evaluate(consideration, unit.Position!.Value, match.GetMap());
            return unit.UnitQuantity.Points 
                * (TileConsiderations.Evaluate(consideration, path.Destination, match.GetMap()) - baseline);
        }

        public static float EvaluatePathMove(
            Unit unit, Pathing.PathOption path, Pathing.Path goal, UnitTileEvaluator tileEvaluator, Match match)
        {
            if (goal.Steps.Contains(path.Destination))
            {
                return EvaluateFreeMove(unit, path, tileEvaluator, match) 
                    + s_PathReward * (path.Cost / unit.Type.Speed);
            }
            return EvaluateFreeMove(unit, path, tileEvaluator, match);
        }
    }
}
