using Cardamom.Collections;
using Expeditionary.Model.Units;

namespace Expeditionary.Evaluation
{
    public static class DefaultDispositionMapper
    {
        private static readonly EnumSet<UnitTag> _defensiveTags =
            new(
                UnitTag.Artillery, 
                UnitTag.HQ, 
                UnitTag.Labor,
                UnitTag.Maintenance,
                UnitTag.Medical,
                UnitTag.MineClearing,
                UnitTag.MineLaunching,
                UnitTag.MineLaunching,
                UnitTag.Mortar,
                UnitTag.Recovery,
                UnitTag.Sensor, 
                UnitTag.Transport);

        public static Disposition Map(UnitType unitType)
        {
            return unitType.GetTags().Any(_defensiveTags.Contains) ? Disposition.Defensive : Disposition.Offensive;
        }
    }
}
