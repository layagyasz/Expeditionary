using Expeditionary.Model.Combat.Units;

namespace Expeditionary.Model.Combat
{
    public static class CombatCalculator
    {
        public static CombatPreview GetPreview(UnitType attacker, UnitAttack attack, UnitType defender, float range)
        {
            // TODO -- derive conditions from tiles
            var combatCondition = CombatCondition.None;
            if (range > attack.Range.GetValue())
            {
                return new();
            }
            var volume = (attack.Volume + attacker.Capabilities.GetVolume(combatCondition)).GetValue();
            var target =
                GetPreviewLayer(
                    (attack.Accuracy + attacker.Capabilities.GetAccuracy(combatCondition)).GetValue() 
                        * (1f - 1f / (attack.Range.GetValue() + 1)), 
                    defender.Intrinsics.Profile.GetValue(),
                    defenseMin: 0,
                    scale: 1);
            var hit =
                GetPreviewLayer(
                    attack.Tracking.GetValue(),
                    defender.Defense.Maneuver.Value.GetValue(),
                    defender.Defense.Maneuver.Minimum.GetValue(),
                    scale: 2);
            var pen =
                GetPreviewLayer(
                    attack.Penetration.GetValue(),
                    defender.Defense.Armor.Value.GetValue(),
                    defender.Defense.Armor.Minimum.GetValue(),
                    scale: 2);
            var kill =
                GetPreviewLayer(
                    (attack.Lethality + attacker.Capabilities.GetLethality(combatCondition)).GetValue(),
                    defender.Defense.Vitality.Value.GetValue(),
                    defender.Defense.Vitality.Minimum.GetValue(),
                    scale: 1);
            return new(
                volume,
                target, 
                hit, 
                pen, 
                kill, 
                volume * target.Probability * hit.Probability * pen.Probability * kill.Probability);
        }

        public static CombatPreview.Layer GetPreviewLayer(float attack, float defense, float defenseMin, float scale)
        {
            return new(attack, defense, defenseMin, GetProbability(attack, defense, defenseMin, scale));
        }

        public static float GetProbability(float attack, float defense, float defenseMin, float scale)
        {
            var raw = scale * (attack - defenseMin) / (attack + defense - 2 * defenseMin);
            if (raw < 0)
            {
                return 0;
            }
            if (raw > 1)
            {
                return 1;
            }
            return raw;
        }
    }
}
