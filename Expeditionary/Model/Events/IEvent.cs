namespace Expeditionary.Model.Events
{
    public interface IEvent
    {
        EventStatus Fire(Match match, TurnInfo turn);
    }
}
