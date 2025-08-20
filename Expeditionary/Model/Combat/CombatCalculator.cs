using Expeditionary.Hexagons;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Units;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Combat
{
    public static class CombatCalculator
    {
        public static bool IsValidLineOfSight(UnitWeapon.Mode mode, Map map, Vector3i position, Vector3i target)
        {
            return mode.IsIndirect() || Sighting.IsValidLineOfSight(map, position, target);
        }

        public static IEnumerable<Sighting.LineOfSight> GetValidAttackHexes(
            UnitWeapon.Mode mode, Map map, Vector3i position)
        {
            var range = mode.IsIndirect()
                ? Sighting.GetSightField(map, position, (int)mode.Range.GetMaximum()) 
                : Sighting.GetUnblockedSightField(map, position, (int)mode.Range.GetMaximum());
            var min = mode.Range.GetMinimum();
            return min > 0 ? range.Where(x => x.Distance >= min) : range;
        }

        public static bool IsValidTarget(Unit attacker, Unit target)
        {
            if (target.IsDestroyed)
            {
                return false;
            }
            if (!target.Position.HasValue)
            {
                return false;
            }
            if (attacker.Player.Team == target.Player.Team)
            {
                return false;
            }
            if (attacker.IsPassenger)
            {
                return false;
            }
            if (target.IsPassenger)
            {
                return false;
            }
            return true;
        }

        public static bool IsValidTarget(Unit attacker, UnitWeapon.Mode mode, Unit target, Map map)
        {
            return IsValidTarget(attacker, attacker.Position!.Value, mode, target, target.Position!.Value, map);
        }

        public static bool IsValidTarget(
            Unit attacker,
            Vector3i attackerPosition,
            UnitWeapon.Mode mode, 
            Unit target, 
            Vector3i targetPosition,
            Map map)
        {
            if (!IsValidTarget(attacker, target))
            {
                return false;
            }
            var distance = Geometry.GetCubicDistance(attackerPosition, targetPosition);
            if (distance > mode.Range.GetMaximum() || distance < mode.Range.GetMinimum())
            {
                return false;
            }
            if (!IsValidLineOfSight(mode, map, attackerPosition, targetPosition))
            {
                return false;
            }
            return true;
        }

        public static CombatPreview GetDirectPreview(
            Unit attacker, UnitWeaponUsage weapon, UnitWeapon.Mode mode, Unit target, Map map)
        {
            return GetDirectPreview(
                attacker, attacker.Position!.Value, weapon, mode, target, target.Position!.Value, map);
        }

        public static CombatPreview GetDirectPreview(
            Unit attacker, 
            Vector3i attackerPosition,
            UnitWeaponUsage attack,
            UnitWeapon.Mode mode, 
            Unit defender,
            Vector3i defenderPosition,
            Map map)
        {
            float range = Geometry.GetCubicDistance(attackerPosition, defenderPosition);
            return GetDirectPreview(
                attacker.Type,
                mode,
                defender.Type,
                GetConditions(mode, range, map.Get(defenderPosition)!),
                range,
                attacker.Number * attack.Number);
        }

        private static CombatPreview GetDirectPreview(
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
