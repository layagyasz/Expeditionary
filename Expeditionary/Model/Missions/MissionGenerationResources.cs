using Expeditionary.Model.Formations.Generator;
using Expeditionary.Model.Mapping.Environments.Generator;

namespace Expeditionary.Model.Missions
{
    public record class MissionGenerationResources(
        MapEnvironmentGenerator EnvironmentGenerator, FormationGenerator FormationGenerator, Random Random);
}
