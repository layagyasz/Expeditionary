using Cardamom.Collections;
using Expeditionary.Model.Units;

namespace Expeditionary.Model.Formations.Generator
{
    public record class UnitConstraint(
        FormationRole Role, EnumSet<UnitTag> RequiredTags, EnumSet<UnitTag> ExcludedTags)
    {
        public bool Satisfies(UnitSlot slot, IEnumerable<UnitTag> tags)
        {
            return Role != slot.Role || (RequiredTags.IsSubsetOf(tags) && !ExcludedTags.Intersect(tags).Any());
        }
    }
}
