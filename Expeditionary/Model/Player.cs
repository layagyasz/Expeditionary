using Expeditionary.Model.Factions;

namespace Expeditionary.Model
{
    public record class Player(int Id, int Team, Faction Faction)
    {
        public bool MatchPlayer(Player other)
        {
            return Id == other.Id;
        }

        public bool MatchTeam(Player other)
        {
            return Team == other.Team;
        }
    }
}
