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
            var preview =
                CombatCalculator.GetPreview(
                    attacker, attacker.Position!.Value, attack, mode, defender, defender.Position!.Value, map);
            return defender.Type.Points * Math.Min(1, preview.Result / defender.Number);
        }
    }
}
