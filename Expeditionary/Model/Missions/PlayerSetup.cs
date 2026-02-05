using Expeditionary.Model.Knowledge;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Missions.Objectives;

namespace Expeditionary.Model.Missions
{
    public record class PlayerSetup(Player Player, IObjective Objective, List<FormationSetup> Formations)
    {
        public void Create(Match match, CreationContext context)
        {
            match.Add(Player, Objective, CreatePlayerKnowledge(Player, match.GetMap(), context));
        }

        public void Setup(Match match, SetupContext context) 
        {
            foreach (var formation in Formations)
            {
                formation.Setup(Player, match, context);
            }
        }

        private IPlayerKnowledge CreatePlayerKnowledge(Player player, Map map, CreationContext context)
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
