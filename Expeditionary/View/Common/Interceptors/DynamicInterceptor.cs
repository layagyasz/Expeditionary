using Cardamom.Graphics;

namespace Expeditionary.View.Common.Interceptors
{
    public class DynamicInterceptor : BaseInterceptor
    {
        public long RefreshTime { get; }

        private long _time;

        public DynamicInterceptor(IRenderable child, long refreshTime)
            : base(child)
        {
            RefreshTime = refreshTime;
        }

        public override void Attach() { }

        public override void Intercept(long delta)
        {
            _time += delta;
            if (_time > RefreshTime)
            {
                _time = 0;
                Refresh();
            }
        }
    }
}
