namespace Expeditionary.Model.Events
{
    public interface IEventSchedule
    {
        bool IsDue(TurnInfo turn);

        public record class RecurringEventSchedule(Player? Player, TurnSegment Segment, int Cycle, int Offset) 
            : IEventSchedule
        {
            public bool IsDue(TurnInfo turn)
            {
                return turn.Player == Player && turn.Segment == Segment && (turn.Turn + Offset) % Cycle == 0;
            }
        }

        public record class FixedEventSchedule(TurnInfo Turn)
        {
            public bool Matches(TurnInfo turn)
            {
                return turn == Turn;
            }
        }
    }
}
