using Cardamom.Collections;
using Expeditionary.Model.Formations;
using Expeditionary.Model.Units;

namespace Expeditionary.Evaluation
{
    public static class DefaultDispositionMapper
    {
        private static readonly EnumSet<UnitTag> s_DefensiveTags =
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
            return unitType.GetTags().Any(s_DefensiveTags.Contains) ? Disposition.Defensive : Disposition.Offensive;
        }

        private static readonly EnumSet<FormationRole> s_DefensiveRoles =
            new(FormationRole.Artillery, FormationRole.Tractor, FormationRole.Transport);

        public static Disposition Map(FormationRole role)
        {
            return s_DefensiveRoles.Contains(role) ? Disposition.Defensive : Disposition.Offensive;
        }
    }
}
