namespace Expeditionary
{
    public class EventBuffer<T>
    {
        private readonly Queue<Tuple<object?, T>> _invocations = new();
        private readonly Action<object?, T> _handler;

        public EventBuffer(Action<object?, T> handler)
        {
            _handler = handler;
        }

        public void QueueEvent(object? sender, T e)
        {
            lock (_invocations)
            {
                _invocations.Enqueue(new Tuple<object?, T>(sender, e));
            }
        }

        public void DispatchEvents()
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
