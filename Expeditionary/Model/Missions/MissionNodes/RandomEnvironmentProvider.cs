using Cardamom.Json;
using Expeditionary.Model.Mapping.Environments;
using Expeditionary.Model.Sectors;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Expeditionary.Model.Missions.MissionNodes
{
    public record class RandomEnvironmentProvider : IEnvironmentProvider
    {
        [JsonConverter(typeof(ReferenceJsonConverter))]
        public SectorNaming? SectorNaming { get; set; }
        public ImmutableList<int> Sectors { get; set; } = ImmutableList.Create<int>();


        public MapEnvironmentDefinition Get(MissionGenerationResources resources)
        {
            var random = resources.Random;
            // TODO: Configure cap and toString() implementation
            var key = 
                new PlanetKey(
                    Sectors[random.Next(Sectors.Count)],
                    random.Next(SectorNaming!.StarPrefixes.Count), 
                    random.Next(10) + 1);
            return resources.EnvironmentGenerator.Generate(key, environment: 0, SectorNaming!);
        }
    }
}
