using Cardamom.Collections;

namespace Expeditionary.Model.Combat.Units
{
    public record class UnitConditionCapabilities(
        Modifier Volume,
        Modifier Accuracy,
        Modifier Lethality,
        EnumMap<UnitDetectionBand, Modifier> Detection,
        EnumMap<UnitDetectionBand, UnitBoundedValue> Concealment);
}
