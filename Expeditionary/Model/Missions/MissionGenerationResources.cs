using Expeditionary.Model.Formations.Generator;

namespace Expeditionary.Model.Missions
{
    public record class MissionGenerationResources(FormationGenerator FormationGenerator, Random Random);
}
