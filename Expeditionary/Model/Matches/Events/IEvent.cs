namespace Expeditionary.Model.Matches.Events
{
    public interface IEvent
    {
        EventStatus Fire(Match match, TurnInfo turn);
    }
}
