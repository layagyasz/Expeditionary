namespace Expeditionary.Events
{
    public interface IEventBuffer<T> : IEventDispatchable
    {
        void Queue(object? sender, T e);
    }
}
