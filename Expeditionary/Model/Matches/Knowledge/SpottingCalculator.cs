using Cardamom.Collections;
using Expeditionary.Model.Matches.Assets;
using Expeditionary.Model.Matches.Combat;
using Expeditionary.Model.Units;

namespace Expeditionary.Model.Matches.Knowledge
{
    public static class SpottingCalculator
    {
        public static bool IsSpotted(
            EnumMap<UnitDetectionBand, float> detection,
            CombatCondition condition,
            IMatchAsset target)
        {
            return detection.Keys.Any(
                x => IsSpotted(
                    detection[x], 
                    GetSignature(target, x, condition), 
                    GetConcealment(target, x, condition)));
        }

        public static bool IsSpotted(float detection, float signature, float concealment)
        {
            return SkillCalculator.SignatureAttenuate(detection, signature) > concealment;
        }

        public static float GetConcealment(IMatchAsset asset, UnitDetectionBand band, CombatCondition conditions)
        {
            if (asset is not MatchUnit unit)
            {
                return 0;
            }
            return unit.Type.Capabilities.GetConcealment(conditions, band).GetValue();
        }

        public static EnumMap<UnitDetectionBand, float> GetDetection(
            MatchUnit detector, Sighting.LineOfSight los, CombatCondition condition)
        {
            return Enum.GetValues<UnitDetectionBand>()
                .ToEnumMap(x => x, x => GetDetection(detector, x, los, condition));
        }

        public static float GetDetection(
            MatchUnit detector, UnitDetectionBand band, Sighting.LineOfSight los, CombatCondition condition)
        {
            if (los.IsBlocked && (band == UnitDetectionBand.Visual || band == UnitDetectionBand.Thermal))
            {
                return 0f;
            }
            return SkillCalculator.RangeAttenuate(
                detector.Type.Capabilities.GetDetection(condition, band).GetValue(),
                detector.Type.Capabilities.GetRange(condition, band).GetValue(),
                los.Distance);
        }

        public static float GetSignature(IMatchAsset asset, UnitDetectionBand band, CombatCondition condition)
        {
            if (asset is not MatchUnit unit)
            {
                return 0;
            }
            return unit.Type.Capabilities.GetSignature(condition, band).GetValue();
        }
    }
}
