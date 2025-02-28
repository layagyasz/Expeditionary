using Cardamom.Collections;

namespace Expeditionary.Model.Units
{
    public record class UnitConditionCapabilities(
        Modifier Volume,
        Modifier Accuracy,
        Modifier Lethality,
        EnumMap<UnitDetectionBand, Modifier> Detection,
        EnumMap<UnitDetectionBand, Modifier> Range,
        EnumMap<UnitDetectionBand, Modifier> Concealment,
        EnumMap<UnitDetectionBand, Modifier> Signature);
}
