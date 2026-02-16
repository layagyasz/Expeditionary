namespace Expeditionary.Events
{
    public class SimpleEventBuffer : IEventBuffer
    {
        private record struct Invocation(EventHandler<object> Handler, object? Sender, object Args); 

        readonly Queue<Invocation> _invocations = new();

        public EventHandler<T> Hook<T>(EventHandler<T> Handler)
        {
            return (Sender, E) => Queue(Handler, Sender, E!);
        }

        public void DispatchEvents(long delta)
        {
            lock (_invocations)
            {
                foreach (var invocation in _invocations)
                {
                    Console.WriteLine(invocation);
                    invocation.Handler(invocation.Sender, invocation.Args);
                }
                _invocations.Clear();
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
                _invocations.Enqueue(new((s, e) => Handler(s, (T)e), Sender, Args!));
            }
        }
    }
}
