namespace Expeditionary.Model
{
    public record struct TurnInfo(int Turn, Player? Player, TurnSegment Segment);
}
