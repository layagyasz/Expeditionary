using Expeditionary.Model.Mapping;
using Expeditionary.Model.Matches;
using Expeditionary.Model.Matches.Events;
using Expeditionary.Model.Matches.Knowledge;
using Expeditionary.Model.Missions.Objectives;

namespace Expeditionary.Model.Missions
{
    public record class PlayerSetup(
        Player Player, IObjective Objective, List<IEvent> Events, List<FormationSetup> Formations)
    {
        public void Create(Match match, CreationContext context)
        {
            match.Add(Player, Objective, CreatePlayerKnowledge(Player, match.GetMap(), context));
        }

        public void Setup(Match match, SetupContext context) 
        {
            foreach (var @event in Events)
            {
                match.Add(@event);
            }
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
