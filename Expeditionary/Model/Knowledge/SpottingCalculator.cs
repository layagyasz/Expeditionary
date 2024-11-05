using Expeditionary.Hexagons;
using Expeditionary.Model.Combat;
using Expeditionary.Model.Combat.Units;
using Expeditionary.Model.Mapping;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Knowledge
{
    public static class SpottingCalculator
    {
        public static bool IsSpotted(Map map, Unit spotter, Unit target, Vector3i hex)
        {
            var targetTile = map.GetTile(hex)!;
            var condition = targetTile.GetConditions();
            return Enum.GetValues<UnitDetectionBand>()
                .Any(x => 
                    IsSpotted(
                        GetDetection(spotter, x, condition, hex), 
                        GetSignature(target, x, condition),
                        GetConcealment(target, x, condition)));
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
