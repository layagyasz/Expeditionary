using Expeditionary.Model.Mapping.Environments;

namespace Expeditionary.Model.Missions.MissionNodes
{
    public interface IEnvironmentProvider
    {
        MapEnvironmentDefinition Get(MissionGenerationResources resources);
    }
}
