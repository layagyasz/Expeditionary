using Cardamom.Collections;

namespace Expeditionary.Model.Combat
{
    public class UnitCapabilities
    {
        public EnumMap<UnitDetectionBand, UnitBoundedValue> Detection { get; set; } = new();
        public EnumMap<UnitDetectionBand, UnitBoundedValue> Concealment { get; set; } = new();
    }
}
