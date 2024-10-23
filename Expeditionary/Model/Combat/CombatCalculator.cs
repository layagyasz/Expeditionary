using Expeditionary.Hexagons;
using Expeditionary.Model.Combat.Units;
using Expeditionary.Model.Mapping;

namespace Expeditionary.Model.Combat
{
    public static class CombatCalculator
    {
        public static CombatPreview GetPreview(Unit attacker, UnitAttack attack, Unit defender, Map map)
        {
            float range = Geometry.GetCubicDistance(attacker.Position, defender.Position);
            if (range > attack.Range.GetValue())
            {
                return new();
            }
            return GetPreview(
                attacker.Type, 
                attack, 
                defender.Type,
                GetConditions(attack, range, map.GetTile(attacker.Position)!, map.GetTile(defender.Position)!),
                range,
                attacker.GetAttackNumber(attack));
        }

        private static CombatPreview GetPreview(
            UnitType attacker, 
            UnitAttack attack, 
            UnitType defender, 
            CombatCondition condition,
            float range,
            float number)
        {
            var volume = number * (attack.Volume + attacker.Capabilities.GetVolume(condition)).GetValue();
            var target =
                GetPreviewLayer(
                    (attack.Accuracy + attacker.Capabilities.GetAccuracy(condition)).GetValue() 
                        * (1f - range / (attack.Range.GetValue() + 1)), 
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
                    (attack.Lethality + attacker.Capabilities.GetLethality(condition)).GetValue(),
                    defender.Defense.Vitality.Value.GetValue(),
                    defender.Defense.Vitality.Minimum.GetValue(),
                    scale: 1);
            return new(
                condition,
                volume,
                target, 
                hit, 
                pen, 
                kill, 
                volume * target.Probability * hit.Probability * pen.Probability * kill.Probability);
        }

        private static CombatCondition GetConditions(UnitAttack attack, float range, Tile attacker, Tile defender)
        {
            CombatCondition condition = attack.Condition;
            if (range < 1)
            {
                condition |= CombatCondition.Close;
            }
            if (IsUrban(attacker.Structure) && IsUrban(defender.Structure))
            {
                condition |= CombatCondition.Urban;
            }
            return condition;
        }

        private static bool IsUrban(Structure structure)
        {
            return structure.Type != StructureType.None && structure.Type != StructureType.Agricultural;
        }

        private static CombatPreview.Layer GetPreviewLayer(float attack, float defense, float defenseMin, float scale)
        {
            return new(attack, defense, defenseMin, GetProbability(attack, defense, defenseMin, scale));
        }

        private static float GetProbability(float attack, float defense, float defenseMin, float scale)
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
