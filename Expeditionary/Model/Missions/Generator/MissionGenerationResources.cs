using Expeditionary.Model.Galaxies;
using Expeditionary.Model.Mapping.Environments.Generator;

namespace Expeditionary.Model.Missions.Generator
{
    public record class MissionGenerationResources(
        Galaxy Galaxy,
        MapEnvironmentGenerator EnvironmentGenerator,
        Random Random);
}
