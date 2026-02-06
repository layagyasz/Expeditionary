namespace Expeditionary.Events
{
    public interface IEventDispatchable
    {
        void DispatchEvents(long delta);
    }
}
