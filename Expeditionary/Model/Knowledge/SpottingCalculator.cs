using Cardamom.Collections;
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
            var los = Sighting.GetLineOfSight(map, spotter.Position, hex);
            return Enum.GetValues<UnitDetectionBand>()
                .Any(x => 
                    IsSpotted(
                        GetDetection(spotter, x, los, condition), 
                        GetSignature(target, x, condition),
                        GetConcealment(target, x, condition)));
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

        public static float GetConcealment(IAsset asset, UnitDetectionBand band, CombatCondition conditions)
        {
            if (asset is not Unit unit)
            {
                return 0;
            }
            return unit.Type.Capabilities.GetConcealment(conditions, band).GetValue();
        }

        public static EnumMap<UnitDetectionBand, float> GetDetection(
            Unit detector, LineOfSight los, CombatCondition condition)
        {
            return Enum.GetValues<UnitDetectionBand>()
                .ToEnumMap(x => x, x => GetDetection(detector, x, los, condition));
        }

        public static float GetDetection(
            Unit detector, UnitDetectionBand band, LineOfSight los, CombatCondition condition)
        {
            if (los.IsBlocked && (band == UnitDetectionBand.Visual || band == UnitDetectionBand.Thermal))
            {
                return 0;
            }
            return SkillCalculator.RangeAttenuate(
                detector.Type.Capabilities.GetDetection(condition, band).GetValue(),
                detector.Type.Capabilities.GetRange(condition, band).GetValue(),
                los.Distance);
        }

        public static float GetSignature(IAsset asset, UnitDetectionBand band, CombatCondition condition)
        {
            if (asset is not Unit unit)
            {
                return 0;
            }
            return unit.Type.Capabilities.GetSignature(condition, band).GetValue();
        }
    }
}
