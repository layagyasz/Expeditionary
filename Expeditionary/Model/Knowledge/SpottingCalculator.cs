using Cardamom.Collections;
using Expeditionary.Hexagons;
using Expeditionary.Model.Combat;
using Expeditionary.Model.Combat.Units;
using Expeditionary.Model.Mapping;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Knowledge
{
    public static class SpottingCalculator
    {
        public static bool IsSpotted(Map map, Unit spotter, IAsset target, Vector3i hex)
        {
            var condition = map.GetTile(hex)!.GetConditions();
            if (target is not Unit targetUnit)
            {
                return Enum.GetValues<UnitDetectionBand>().Any(x => GetDetection(spotter, x, condition, hex) > 0);
            }
            return Enum.GetValues<UnitDetectionBand>()
                .Any(x => 
                    IsSpotted(
                        GetDetection(spotter, x, condition, hex), 
                        GetSignature(targetUnit, x, condition),
                        GetConcealment(targetUnit, x, condition)));
        }

        public static bool IsSpotted(
            EnumMap<UnitDetectionBand, float> detection, 
            CombatCondition condition,
            IAsset target)
        {
            if (target is not Unit targetUnit)
            {
                return detection.Any(x => x.Value > 0);
            }
            return detection.Keys.Any(
                x => IsSpotted(
                    detection[x], 
                    GetSignature(targetUnit, x, condition), 
                    GetConcealment(targetUnit, x, condition)));
        }

        public static bool IsSpotted(float detection, float signature, float concealment)
        {
            return SkillCalculator.SignatureAttenuate(detection, signature) >= concealment;
        }

        public static float GetConcealment(Unit unit, UnitDetectionBand band, CombatCondition conditions)
        {
            return unit.Type.Capabilities.GetConcealment(conditions, band).GetValue();
        }

        public static float GetDetection(
            Unit detector, UnitDetectionBand band, CombatCondition condition, Vector3i hex)
        {
            return SkillCalculator.RangeAttenuate(
                detector.Type.Capabilities.GetDetection(condition, band).GetValue(),
                detector.Type.Capabilities.GetRange(condition, band).GetValue(),
                Geometry.GetCubicDistance(detector.Position, hex));
        }

        public static float GetSignature(Unit unit, UnitDetectionBand band, CombatCondition condition)
        {
            return unit.Type.Capabilities.GetSignature(condition, band).GetValue();
        }
    }
}
