namespace Expeditionary.Events
{
    public class SimpleEventBuffer<T> : IEventBuffer<T>
    {
        private readonly Queue<(object?, T)> _invocations = new();
        private readonly Action<object?, T> _handler;

        public SimpleEventBuffer(Action<object?, T> handler)
        {
            _handler = handler;
        }

        public void Queue(object? sender, T e)
        {
            lock (_invocations)
            {
                _invocations.Enqueue((sender, e));
            }
        }

        public void DispatchEvents(long delta)
        {
            lock (_invocations)
            {
                foreach (var invocation in _invocations)
                {
                    _handler(invocation.Item1, invocation.Item2);
                }
                _invocations.Clear();
            }
        }
    }
}
