namespace Expeditionary.Model.Matches
{
    public record struct TurnInfo(int Turn, MatchPlayer? Player, TurnSegment Segment);
}
