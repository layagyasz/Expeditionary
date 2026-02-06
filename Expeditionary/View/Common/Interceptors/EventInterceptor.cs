using Cardamom.Graphics;
using Expeditionary.Events;

namespace Expeditionary.View.Common.Interceptors
{
    public class EventInterceptor : BaseInterceptor
    {
        public IEventDispatchable Dispatch { get; }


        public EventInterceptor(IRenderable child, IEventDispatchable dispatch)
            : base(child)
        {
            Dispatch = dispatch;
        }

        public override void Attach() { }

        public override void Intercept(long delta)
        {
            Dispatch.DispatchEvents(delta);
        }
    }
}
