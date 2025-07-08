using Cardamom.Collections;
using Expeditionary.Model.Units;

namespace Expeditionary.Model.Formations.Generator
{
    public record class UnitSlot
    {
        public FormationRole Role { get; set; }
        public EnumSet<UnitTag> RequiredTags { get; set; } = new();
        public EnumSet<UnitTag> ExcludedTags { get; set; } = new();

        public bool Matches(UnitUsage unitUsage)
        {
            if (unitUsage.Role != Role)
            {
                return false;
            }
            var tags = unitUsage.Type!.GetTags();
            if (RequiredTags.Any() && !RequiredTags.All(tags.Contains))
            {
                return false;
            }
            if (ExcludedTags.Any() && ExcludedTags.Any(tags.Contains))
            {
                return false;
            }
            return true;
        }
    }
}
