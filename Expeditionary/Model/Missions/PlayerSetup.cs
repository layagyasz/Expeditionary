using Expeditionary.Model.Knowledge;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Missions.Objectives;

namespace Expeditionary.Model.Missions
{
    public record class PlayerSetup(Player Player, ObjectiveSet Objectives, List<FormationSetup> Formations)
    {
        public void Setup(Match match, PlayerSetupContext context) 
        {
            match.Add(Player, Objectives, CreatePlayerKnowledge(Player, match.GetMap(), context.Parent));
            foreach (var formation in Formations)
            {
                formation.Setup(Player, match, context);
            }
        }

        private IPlayerKnowledge CreatePlayerKnowledge(Player player, Map map, SetupContext context)
        {
            if (context.Player == player && context.IsTest)
            {
                return new OmniscientPlayerKnowledge(player);
            }
            return new LimitedPlayerKnowledge(
                Player, map, new(Player), new LimitedMapKnowledge(map, new KnownMapDiscovery()));
        }
    }
}
