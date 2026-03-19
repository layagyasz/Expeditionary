namespace Expeditionary.Model.Matches
{
    public record struct TurnInfo(int Turn, Player? Player, TurnSegment Segment);
}
