using Expeditionary.Model.Mapping.Environments;

namespace Expeditionary.Model.Missions.Generator
{
    public interface IEnvironmentProvider
    {
        MapEnvironmentDefinition Get(MissionGenerationResources resources);
    }
}
