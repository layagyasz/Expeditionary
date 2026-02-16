namespace Expeditionary.Events
{
    public interface IEventBuffer : IEventDispatchable
    {
        EventHandler<T> Hook<T>(EventHandler<T> Handler);
        void Queue<T>(EventHandler<T>? Handler, object? Sender, T Args);
    }
}
