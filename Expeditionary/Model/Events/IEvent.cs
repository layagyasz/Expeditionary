namespace Expeditionary.Model.Events
{
    public interface IEvent
    {
        bool IsRecurring { get; }
        bool Fire(Match match, TurnInfo turn);
    }
}
