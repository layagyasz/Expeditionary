using Expeditionary.Model.Factions;

namespace Expeditionary.Model.Matches
{
    public record class MatchPlayer(int Id, int Team, Faction Faction)
    {
        public bool MatchTeam(MatchPlayer other)
        {
            return Team == other.Team;
        }
    }
}
