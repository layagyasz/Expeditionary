using Cardamom.Collections;
using Expeditionary.Model.Combat.Units;

namespace Expeditionary.Model.Combat.Formations.Generator
{
    public record class FormationSlot
    {
        public int Number { get; set; }
        public UnitRole Role { get; set; }
        public EnumSet<UnitTag> RequiredTags { get; set; } = new();
        public EnumSet<UnitTag> ExcludedTags { get; set; } = new();

        public FormationSlot WithTags(IEnumerable<UnitTag> requiredTags, IEnumerable<UnitTag> excludedTags)
        {
            return new()
            {
                Number = Number,
                Role = Role,
                RequiredTags = RequiredTags.Union(requiredTags).ToEnumSet(),
                ExcludedTags = ExcludedTags.Union(excludedTags).ToEnumSet()
            };
        }

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
