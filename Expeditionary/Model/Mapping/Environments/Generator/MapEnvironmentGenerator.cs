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

        public ImmutableList<TraitGroup> Groups { get; set; } = ImmutableList.Create<TraitGroup>();

        public MapEnvironmentDefinition Generate(string name, int seed)
        {
            var random = new Random(seed);
            return new MapEnvironmentDefinition()
            {
                Key = $"map-environment-{seed}",
                Name = name,
                Traits = Groups.Select(x => x.Traits.Get(random.NextSingle())).ToList()
            };
        }
    }
}
