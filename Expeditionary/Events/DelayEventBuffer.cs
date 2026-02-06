using Cardamom.Collections;

namespace Expeditionary.Events
{
    public class DelayEventBuffer<T> : IEventBuffer<T>
    {
        private readonly Heap<(object?, T), long> _invocations = new();
        private readonly Action<object?, T> _handler;
        private readonly long _delay;

        private long _time;

        public DelayEventBuffer(Action<object?, T> handler, long delay)
        {
            _handler = handler;
            _delay = delay;
        }

        public void Queue(object? sender, T e)
        {
            lock (_invocations)
            {
                _invocations.Push((sender, e), _time + _delay);
            }
        }

        public void DispatchEvents(long delta)
        {
            _time += delta;
            lock (_invocations)
            {
                while (_invocations.Count > 0 && _invocations.PeekValue() <= _time)
                {
                    (var sender, var args) = _invocations.Pop();
                    _handler.Invoke(sender, args);
                }
            }
        }
    }
}
