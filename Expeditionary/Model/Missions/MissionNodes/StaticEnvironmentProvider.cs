using Expeditionary.Model.Mapping;

namespace Expeditionary.Model.Missions.MissionNodes
{
    public record class StaticEnvironmentProvider(MapEnvironmentDefinition environment) : IEnvironmentProvider
    {
        public MapEnvironmentDefinition Get(Random random)
        {
            return environment;
        }
    }
}
