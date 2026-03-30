using Expeditionary.Model.Mapping;
using Expeditionary.Model.Matches;
using Expeditionary.Model.Matches.Assets;
using Expeditionary.Model.Matches.Combat;
using Expeditionary.Model.Matches.Evaluation.Considerations;
using Expeditionary.Model.Matches.Evaluation.TileEvaluators;
using Expeditionary.Model.Units;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Matches.Evaluation
{
    public static class ActionEvaluation
    {
        private static readonly float s_PathReward = 20f;

        public static float EvaluateDirectAttack(
            MatchUnit attacker, UnitWeaponUsage attack, UnitWeapon.Mode mode, MatchUnit defender, Map map)
        {
            var preview =
                CombatCalculator.GetDirectPreview(
                    attacker, attacker.Position, attack, mode, defender, defender.Position, map);
            return defender.Type.Points * Math.Min(1, preview.Result / defender.Number);
        }

        public static float EvaluateIndirectAttack(
            MatchUnit attacker, UnitWeaponUsage attack, UnitWeapon.Mode mode, Vector3i target, Match match)
        {
            float result = 0f;
            var map = match.GetMap();
            foreach (var targetAsset in match.GetAssetsAt(target))
            {
                if (targetAsset is MatchUnit targetUnit)
                {
                    var preview =
                        CombatCalculator.GetIndirectPreview(
                            attacker, attacker.Position, attack, mode, targetUnit, target, map);
                    result += (targetUnit.Player.Team == targetUnit.Player.Team ? -1 : 1)
                        * targetUnit.Type.Points * Math.Min(1, preview.Result / targetUnit.Number);
                }
            }
            return result;
        }

        public static float EvaluateMove(
            MatchUnit unit, Pathing.PathOption path, Match match, UnitTileEvaluator tileEvaluator)
        {
            var consideration =
                    tileEvaluator.GetThreatConsiderationFor(Disposition.Defensive, match);
            var baseline = TileConsiderations.Evaluate(consideration, unit.Position, match.GetMap());
            return unit.Value.Points
                * (TileConsiderations.Evaluate(consideration, path.Destination, match.GetMap()) - baseline);
        }

        public static float EvaluateMovePathBonus(MatchUnit unit, Pathing.PathOption path, Pathing.Path goal)
        {
            if (goal.Steps.Contains(path.Destination))
            {
                return s_PathReward * (path.Cost / unit.Type.Speed);
            }
            return 0;
        }

        public static float EvaluateUnload(MatchUnit unit, Match match, UnitTileEvaluator tileEvaluator)
        {
            if (unit.Passenger == null)
            {
                return 0;
            }
            if (unit.Passenger is MatchUnit passengerUnit)
            {
                var baselineConsideration = TileConsiderations.Evaluate(
                    tileEvaluator.GetThreatConsiderationFor(Disposition.Defensive, match),
                    unit.Position,
                    match.GetMap());
                var baseline = (unit.Value.Points + unit.Passenger.Value.Points) * baselineConsideration;
                var passenger =
                    TileConsiderations.Evaluate(
                        tileEvaluator
                            .ForPassenger(passengerUnit)
                            .GetThreatConsiderationFor(Disposition.Defensive, match),
                        unit.Position,
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
