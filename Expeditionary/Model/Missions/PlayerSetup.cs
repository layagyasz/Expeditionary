using Expeditionary.Model.Mapping;
using Expeditionary.Model.Matches;
using Expeditionary.Model.Matches.Events;
using Expeditionary.Model.Matches.Knowledge;
using Expeditionary.Model.Missions.Objectives;

namespace Expeditionary.Model.Missions
{
    public record class PlayerSetup(Player Player, IObjective Objective, List<IEvent> Events, FormationSetup Formation)
    {
        public void Create(Match match, CreationContext context)
        {
            match.Add(Player, Objective, CreatePlayerKnowledge(Player, match.GetMap(), context));
        }

        public void Setup(Match match, SetupContext context) 
        {
            var playerContext = context.GetPlayerContext(Player);
            if (!playerContext.IsHuman)
            {
                context.AiManager.Add(Player);
            }
            foreach (var @event in Events)
            {
                match.Add(@event);
            }
            Formation.Setup(Player, match, context);
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
