using Expeditionary.Model.Mapping;

namespace Expeditionary.Model.Missions.MissionNodes
{
    public interface IEnvironmentProvider
    {
        MapEnvironmentDefinition Get(Random random);
    }
}
