using Cardamom.Collections;
using Expeditionary.Model.Combat.Units;

namespace Expeditionary.Model.Combat.Formations.Generator
{
    public class FormationGeneratorContext
    {
        public IIdGenerator IdGenerator { get; }
        public Random Random { get; }
        public EnumSet<UnitTag> RequiredTags { get; }
        public EnumSet<UnitTag> ExcludedTags { get; }
        public List<UnitUsage> AvailableUnits { get; }

        public FormationGeneratorContext(
            IIdGenerator idGenerator,
            Random random, 
            EnumSet<UnitTag> requiredTags, 
            EnumSet<UnitTag> excludedTags, 
            List<UnitUsage> availableUnits)
        {
            IdGenerator = idGenerator;
            Random = random;
            RequiredTags = requiredTags;
            ExcludedTags = excludedTags;
            AvailableUnits = availableUnits;
        }

        public FormationGeneratorContext WithTags(IEnumerable<UnitTag> requiredTags, IEnumerable<UnitTag> excludedTags)
        {
            return new(
                IdGenerator,
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
