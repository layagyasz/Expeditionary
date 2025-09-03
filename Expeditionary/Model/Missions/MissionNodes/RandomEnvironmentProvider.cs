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
            var name = GenerateName(resources.Random);
            return resources.EnvironmentGenerator.Generate(name, resources.Random.Next());
        }

        private string GenerateName(Random random)
        {
            var sector = Sectors[random.Next(Sectors.Count)];
            var star = random.Next(SectorNaming!.StarPrefixes.Count);
            // TODO: Configure cap and toString() implementation
            var body = random.Next(10) + 1;
            return $"{SectorNaming.StarPrefixes[star]} {SectorNaming.Sectors[sector].StarName} {body}";
        }
    }
}
