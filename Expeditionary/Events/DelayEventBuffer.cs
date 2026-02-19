using Cardamom.Collections;

namespace Expeditionary.Events
{
    public class DelayEventBuffer : IEventBuffer
    {
        private record struct Invocation(EventHandler<object> Handler, object? Sender, object Args);

        private readonly Heap<Invocation, long> _invocations = new();
        private readonly long _delay;

        private long _time;

        public DelayEventBuffer(long delay)
        {
            _delay = delay;
        }

        public EventHandler<T> Hook<T>(EventHandler<T> Handler)
        {
            return (sender, e) => Queue(Handler, sender, e!);
        }

        public void DispatchEvents(long delta)
        {
            _time += delta;
            lock (_invocations)
            {
                while (_invocations.Count > 0 && _invocations.PeekValue() <= _time)
                {
                    var invocation = _invocations.Pop();
                    invocation.Handler(invocation.Sender, invocation.Args);
                }
            }
        }

        public void Queue<T>(EventHandler<T>? Handler, object? Sender, T Args)
        {
            if (Handler == null)
            {
                return;
            }
            lock (_invocations)
            {
                _invocations.Push(new((s, e) => Handler?.Invoke(s, (T)e), Sender, Args!), _delay);
            }
        }
    }
}
