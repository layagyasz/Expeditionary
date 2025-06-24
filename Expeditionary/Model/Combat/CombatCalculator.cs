using Expeditionary.Hexagons;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Units;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Combat
{
    public static class CombatCalculator
    {
        public static bool IsValidTarget(Unit attacker, Unit defender)
        {
            if (defender.IsDestroyed)
            {
                return false;
            }
            if (!defender.Position.HasValue)
            {
                return false;
            }
            if (attacker.Player.Team == defender.Player.Team)
            {
                return false;
            }
            return true;
        }

        public static bool IsValidTarget(Unit attacker, UnitWeapon.Mode mode, Unit defender, Map map)
        {
            return IsValidTarget(attacker, attacker.Position!.Value, mode, defender, defender.Position!.Value, map);
        }

        public static bool IsValidTarget(
            Unit attacker,
            Vector3i attackerPosition,
            UnitWeapon.Mode mode, 
            Unit defender, 
            Vector3i defenderPosition,
            Map map)
        {
            if (!IsValidTarget(attacker, defender))
            {
                return false;
            }
            if (Geometry.GetCubicDistance(attackerPosition, defenderPosition) > mode.Range.Get())
            {
                return false;
            }
            if (!Sighting.IsValidLineOfSight(map, attackerPosition, defenderPosition))
            {
                return false;
            }
            return true;
        }

        public static CombatPreview GetPreview(
            Unit attacker, UnitWeaponUsage weapon, UnitWeapon.Mode mode, Unit defender, Map map)
        {
            return GetPreview(
                attacker, attacker.Position!.Value, weapon, mode, defender, defender.Position!.Value, map);
        }

        public static CombatPreview GetPreview(
            Unit attacker, 
            Vector3i attackerPosition,
            UnitWeaponUsage attack,
            UnitWeapon.Mode mode, 
            Unit defender,
            Vector3i defenderPosition,
            Map map)
        {
            float range = Geometry.GetCubicDistance(attackerPosition, defenderPosition);
            if (range > mode.Range.Get())
            {
                return new();
            }
            return GetPreview(
                attacker.Type,
                mode,
                defender.Type,
                GetConditions(mode, range, map.Get(defenderPosition)!),
                range,
                attacker.Number * attack.Number);
        }

        private static CombatPreview GetPreview(
            UnitType attacker, 
            UnitWeapon.Mode mode,
            UnitType defender, 
            CombatCondition condition,
            float range,
            float number)
        {
            var volume = number * (mode.Volume + attacker.Capabilities.GetVolume(condition)).GetValue();
            var target =
                GetPreviewLayer(
                    SkillCalculator.RangeAttenuate(
                        (mode.Accuracy + attacker.Capabilities.GetAccuracy(condition)).GetValue(),
                        mode.Range.Targeting.GetValue(), 
                        range),
                    defender.Intrinsics.Profile.GetValue(),
                    defenseMin: 0,
                    scale: 1);
            var hit =
                GetPreviewLayer(
                    mode.Tracking.GetValue(),
                    defender.Defense.Maneuver.Value.GetValue(),
                    defender.Defense.Maneuver.Minimum.GetValue(),
                    scale: 2);
            var pen =
                GetPreviewLayer(
                    mode.Penetration.GetValue(),
                    defender.Defense.Armor.Value.GetValue(),
                    defender.Defense.Armor.Minimum.GetValue(),
                    scale: 2);
            var kill =
                GetPreviewLayer(
                    (mode.Lethality + attacker.Capabilities.GetLethality(condition)).GetValue(),
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

        private static CombatCondition GetConditions(UnitWeapon.Mode mode, float range, Tile defender)
        {
            CombatCondition condition = mode.Condition;
            if (range < 1)
            {
                condition |= CombatCondition.Close;
            }
            condition |= defender.GetConditions();
            return condition;
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
