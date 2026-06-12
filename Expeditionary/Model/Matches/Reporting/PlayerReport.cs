using Expeditionary.Model.Missions.Objectives;

namespace Expeditionary.Model.Matches.Reporting
{
    public record class PlayerReport(
        ObjectiveStatus ObjectiveStatus, PlayerStatistics Statistics, FormationReport Formation)
    {
        public static PlayerReport Generate(MatchPlayer player, Match match)
        {
            return new(
                match.GetObjectiveStatus(player),
                match.GetStatistics(player),
                // TODO: Each MatchPlayer should get only a single formation
                FormationReport.Generate(match.GetFormations(player).First()));
        }
    }
}
