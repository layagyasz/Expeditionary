using Cardamom.Collections;
using Expeditionary.Model.Units;

namespace Expeditionary.Model.Formations.Generator
{
    public class FormationGeneratorContext
    {
        public Random Random { get; }
        public EnumSet<UnitTag> RequiredTags { get; }
        public EnumSet<UnitTag> ExcludedTags { get; }
        public List<UnitUsage> AvailableUnits { get; }

        public FormationGeneratorContext(
            Random random, 
            EnumSet<UnitTag> requiredTags, 
            EnumSet<UnitTag> excludedTags, 
            List<UnitUsage> availableUnits)
        {
            Random = random;
            RequiredTags = requiredTags;
            ExcludedTags = excludedTags;
            AvailableUnits = availableUnits;
        }

        public FormationGeneratorContext WithTags(IEnumerable<UnitTag> requiredTags, IEnumerable<UnitTag> excludedTags)
        {
            return new(
                Random, 
                RequiredTags.Union(requiredTags).ToEnumSet(),
                ExcludedTags.Union(excludedTags).ToEnumSet(),
                AvailableUnits);
        }

        public UnitType Select(FormationSlot slot)
        {
            var matchingUnits = AvailableUnits.Where(slot.Matches).ToList();
            return matchingUnits[Random.Next(matchingUnits.Count)].Type!;
        }
    }
}
