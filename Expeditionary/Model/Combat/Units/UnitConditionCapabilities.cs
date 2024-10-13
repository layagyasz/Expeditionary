using Cardamom.Collections;

namespace Expeditionary.Model.Combat.Units
{
    public class UnitConditionCapabilities
    {
        private readonly Modifier _volume;
        private readonly Modifier _accuracy;
        private readonly Modifier _lethality;
        private readonly EnumMap<UnitDetectionBand, Modifier> _detection;
        private readonly EnumMap<UnitDetectionBand, UnitBoundedValue> _concealment;

        public UnitConditionCapabilities(
            Modifier volume,
            Modifier accuracy,
            Modifier lethality,
            EnumMap<UnitDetectionBand, Modifier> detection,
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
