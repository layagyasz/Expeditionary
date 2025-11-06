using Expeditionary.Model.Galaxies;
using Expeditionary.Model.Mapping.Environments;
using System.Collections.Immutable;

namespace Expeditionary.Model.Missions.Generator
{
    public record class RandomEnvironmentProvider : IEnvironmentProvider
    {
        public ImmutableList<int> Sectors { get; set; } = ImmutableList.Create<int>();

        public MapEnvironmentDefinition Get(MissionGenerationResources resources)
        {
            var random = resources.Random;
            // TODO: Configure cap and toString() implementation
            var sector = Sectors[random.Next(Sectors.Count)];
            var key = 
                new MapEnvironmentKey(
                    Sectors[random.Next(Sectors.Count)],
                    random.Next(resources.Galaxy.Sectors[sector].SystemCount), 
                    random.Next(10) + 1,
                    Environment: 0);
            return resources.EnvironmentGenerator.Generate(key);
        }
    }
}
