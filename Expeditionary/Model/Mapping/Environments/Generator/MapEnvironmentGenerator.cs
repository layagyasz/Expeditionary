using Cardamom.Collections;
using Cardamom.Json.Collections;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Expeditionary.Model.Mapping.Environments.Generator
{
    public class MapEnvironmentGenerator
    {
        public class TraitGroup
        {
            [JsonConverter(typeof(ReferenceDictionaryJsonConverter))]
            public WeightedVector<MapEnvironmentTrait> Traits { get; set; } = new();
        }

        public class TraitRule
        {
            [JsonConverter(typeof(ReferenceCollectionJsonConverter))]
            public HashSet<MapEnvironmentTrait> Traits { get; set; } = new();

            [JsonConverter(typeof(ReferenceDictionaryJsonConverter))]
            public WeightedVector<MapEnvironmentTrait> Links { get; set; } = new();
        }

        public ImmutableList<TraitGroup> Groups { get; set; } = ImmutableList.Create<TraitGroup>();
        public ImmutableList<TraitRule> Rules { get; set; } = ImmutableList.Create<TraitRule>();

        public MapEnvironmentDefinition Generate(string name, int seed)
        {
            var random = new Random(seed);
            var result = new HashSet<MapEnvironmentTrait>();
            foreach (var group in Groups)
            {
                var adjusted = GetAdjustedTraits(group.Traits, Rules, result);
                result.Add(adjusted.Get(random.NextSingle()));
            }
            return new MapEnvironmentDefinition()
            {
                Key = $"map-environment-{seed}",
                Name = name,
                Traits = result.ToList()
            };
        }

        private static WeightedVector<MapEnvironmentTrait> GetAdjustedTraits(
            WeightedVector<MapEnvironmentTrait> optionTraits,
            IEnumerable<TraitRule> rules, 
            ISet<MapEnvironmentTrait> selectedTraits)
        {
            var underConsideration = optionTraits.Keys.ToHashSet();
            var overwrites = 
                rules.Where(x => x.Traits.All(selectedTraits.Contains))
                    .SelectMany(x => x.Links)
                    .Where(x => underConsideration.Contains(x.Key))
                    .GroupBy(x => x.Key)
                    .ToDictionary(x => x.Key, x => x.Max(x => x.Value));
            if (!overwrites.Any())
            {
                return optionTraits;
            }
            var result = new WeightedVector<MapEnvironmentTrait>();
            foreach (var overwrite in overwrites)
            {
                result.Add(overwrite);
            }
            if (result.Total >= 1f)
            {
                return result;
            }
            var carryOvers = optionTraits.Where(x => !result.ContainsKey(x.Key)).ToList();
            var scale = (1f - result.Total) / carryOvers.Sum(x => x.Value);
            if (!float.IsNormal(scale))
            {
                return result;
            }
            foreach (var trait in carryOvers)
            {
                result.Add(trait.Key, scale * trait.Value);
            }
            return result;
        }
    }
}
