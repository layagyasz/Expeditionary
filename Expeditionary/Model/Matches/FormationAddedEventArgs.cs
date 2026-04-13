using Expeditionary.Model.Matches.Assets;

namespace Expeditionary.Model.Matches
{
    public record class FormationAddedEventArgs(MatchFormation Formation, MatchFormation? Parent);
}
