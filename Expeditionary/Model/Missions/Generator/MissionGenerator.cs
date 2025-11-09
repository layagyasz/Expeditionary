using Expeditionary.Model.Formations.Generator;
using Expeditionary.Model.Galaxies;
using Expeditionary.Model.Mapping.Environments.Generator;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Missions.Generator
{
    public record class MissionGenerator(
        Galaxy Galaxy, MapEnvironmentGenerator Environments, FormationGenerator Formations)
    {
        public Mission Generate(MissionNode node, long time, int seed)
        {
            var resources = new MissionGenerationResources(Galaxy, Environments, Formations, new(seed));
            var content = node.Content.Generate(node, resources);
            var sector = resources.Galaxy.Sectors[content.Map.Environment.Location.Sector];
            var random = new Random(content.Map.Environment.Location.SystemSeed());
            return new(
                time,
                (long)node.Duration.Generate(random),
                sector.TopLeft + sector.Size * new Vector2(random.NextSingle(), random.NextSingle()),
                content);
        }
    }
}
