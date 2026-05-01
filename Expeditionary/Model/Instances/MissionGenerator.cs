using Expeditionary.Model.Galaxies;
using Expeditionary.Model.Instances.Campaigns;
using Expeditionary.Model.Mapping.Environments.Generator;
using Expeditionary.Model.Missions.Generator;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Instances
{
    public record class MissionGenerator(Galaxy Galaxy, MapEnvironmentGenerator Environments)
    {
        public InstanceMission Generate(CampaignStageKey campaignStageKey, MissionNode node, long time, int seed)
        {
            var resources = new MissionGenerationResources(Galaxy, Environments, new(seed));
            var content = node.Content.Generate(node, resources);
            var sector = Galaxy.Sectors[content.Map.Environment.Location.Sector];
            var random = new Random(content.Map.Environment.Location.SystemSeed());
            return new(
                campaignStageKey,
                time,
                time + (long)node.Duration.Generate(random),
                sector.TopLeft + sector.Size * new Vector2(random.NextSingle(), random.NextSingle()),
                content);
        }
    }
}
