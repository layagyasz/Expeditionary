using Cardamom.Graphics;
using Cardamom.Ui;
using Expeditionary.View.Common.Components.Dynamics;
using OpenTK.Mathematics;

namespace Expeditionary.View.Common.Interceptors
{
    public abstract class BaseInterceptor : IDynamic
    {
        public EventHandler<EventArgs>? Refreshed { get; set; }

        public IRenderable Child { get; }

        public BaseInterceptor(IRenderable child)
        {
            Child = child;
        }

        public abstract void Intercept(long delta);
        public abstract void Attach();

        public void Draw(IRenderTarget target, IUiContext context)
        {
            Child.Draw(target, context);
        }

        public void Initialize()
        {
            Child.Initialize();
            Attach();
        }

        public void Refresh()
        {
            if (Child is IDynamic dynamic)
            {
                dynamic.Refresh();
            }
            Refreshed?.Invoke(this, EventArgs.Empty);
        }

        public void ResizeContext(Vector3 bounds)
        {
            Child.ResizeContext(bounds);
        }

        public void Update(long delta)
        {
            Intercept(delta);
            Child.Update(delta);
        }
    }
}
