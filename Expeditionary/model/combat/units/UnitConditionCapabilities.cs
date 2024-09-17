using Cardamom.Collections;

namespace Expeditionary.model.combat.units
{
    public class UnitConditionCapabilities
    {
        private readonly UnitModifier _volume;
        private readonly UnitModifier _accuracy;
        private readonly UnitModifier _lethality;
        private readonly EnumMap<UnitDetectionBand, UnitModifier> _detection;
        private readonly EnumMap<UnitDetectionBand, UnitBoundedValue> _concealment;

        public UnitConditionCapabilities(
            UnitModifier volume,
            UnitModifier accuracy,
            UnitModifier lethality,
            EnumMap<UnitDetectionBand, UnitModifier> detection,
            EnumMap<UnitDetectionBand, UnitBoundedValue> concealment)
        {
            _volume = volume;
            _accuracy = accuracy;
            _lethality = lethality;
            _detection = detection;
            _concealment = concealment;
        }
    }
}
