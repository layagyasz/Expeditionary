using System.Collections.Immutable;

namespace Expeditionary.Model.Matches.Reporting
{
    public record class MatchReport(ImmutableDictionary<MatchPlayer, PlayerReport> Players)
    {
        public static MatchReport Generate(Match match)
        {
            return new(match.GetPlayers()
                .ToImmutableDictionary(player => player, player => PlayerReport.Generate(player, match)));
        }
    }
}
