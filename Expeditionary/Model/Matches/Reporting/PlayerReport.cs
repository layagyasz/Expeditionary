using Expeditionary.Model.Matches.Assets;
using Expeditionary.Model.Missions.Objectives;
using System.Collections.Immutable;

namespace Expeditionary.Model.Matches.Reporting
{
    public record class PlayerReport(
        ObjectiveStatus ObjectiveStatus, PlayerStatistics Statistics, ImmutableList<UnitReport> Units)
    {
        public static PlayerReport Generate(MatchPlayer player, Match match)
        {
            return new(
                match.GetObjectiveStatus(player),
                match.GetStatistics(player),
                match.GetAssets()
                    .Where(asset => asset is MatchUnit)
                    .Cast<MatchUnit>()
                    .Select(UnitReport.Generate)
                    .ToImmutableList());
        }
    }
}
