using Expeditionary.Model.Matches.Assets;
using System.Collections.Immutable;

namespace Expeditionary.Model.Matches.Reporting
{
    public record class PlayerReport(PlayerStatistics Statistics, ImmutableList<UnitReport> Units)
    {
        public static PlayerReport Generate(MatchPlayer player, Match match)
        {
            return new(
                match.GetStatistics(player),
                match.GetAssets()
                    .Where(asset => asset is MatchUnit)
                    .Cast<MatchUnit>()
                    .Select(UnitReport.Generate)
                    .ToImmutableList());
        }
    }
}
