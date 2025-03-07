using Expeditionary.Model.Knowledge;
using Expeditionary.Model.Missions.Objectives;

namespace Expeditionary.Model.Missions
{
    public record class PlayerSetup(Player Player, ObjectiveSet Objectives, List<FormationSetup> Formations)
    {
        public void Setup(Match match, SetupContext context) 
        {
            match.Add(
                Player,
                Objectives,
                new PlayerKnowledge(
                    Player, match.GetMap(), new(Player), new MapKnowledge(match.GetMap(), new KnownMapDiscovery())));
            foreach (var formation in Formations)
            {
                formation.Setup(Player, match, context);
            }
        }
    }
}
