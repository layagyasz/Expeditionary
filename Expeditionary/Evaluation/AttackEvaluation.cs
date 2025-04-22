using Expeditionary.Hexagons;
using Expeditionary.Model;
using Expeditionary.Model.Combat;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Units;

namespace Expeditionary.Evaluation
{
    public static class AttackEvaluation
    {
        public static float Evaluate(
            Unit attacker, UnitWeaponUsage attack, UnitWeapon.Mode mode, Unit defender, Map map)
        {
            var preview = CombatCalculator.GetPreview(attacker, attack, mode, defender, map);
            return defender.Type.Points * Math.Min(1, preview.Result / defender.Number);
        }

        public static bool IsValidTarget(Unit attacker, UnitWeapon.Mode mode, Unit defender, Map map)
        {
            if (Geometry.GetCubicDistance(attacker.Position!.Value, defender.Position!.Value) > mode.Range.Get())
            {
                return false;
            }
            if (!Sighting.IsValidLineOfSight(map, attacker.Position!.Value, defender.Position!.Value))
            {
                return false;
            }
            return true;
        }
    }
}
