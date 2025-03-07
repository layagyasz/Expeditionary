using Expeditionary.Model.Factions;
using Expeditionary.Model.Mapping;

namespace Expeditionary.Model.Missions
{
    public record class MissionParameters(
        MapEnvironmentDefinition Environment, List<Faction> Attackers, List<Faction> Defenders);
}
