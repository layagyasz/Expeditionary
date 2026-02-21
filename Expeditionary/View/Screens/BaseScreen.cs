using Cardamom;
using Cardamom.Graphics;
using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Expeditionary.View.Common.Components.Dynamics;
using OpenTK.Mathematics;

namespace Expeditionary.View.Screens
{
    public abstract class BaseScreen : ManagedResource, IDynamic, IScreen
    {
        public event EventHandler<EventArgs>? Refreshed;

        public IController Controller { get; }

        private readonly List<IRenderable> _elements = new();

        protected BaseScreen(IController controller)
        {
            Controller = controller;
        }

        protected void Register(IRenderable element)
        {
            _elements.Add(element);
        }

        public virtual void Draw(IRenderTarget target, IUiContext context)
        {
            foreach (var element in _elements)
            {
                element.Draw(target, context);
                if (element is IScene)
                {
                    context.Flatten();
                }
            }
        }

        public virtual void Initialize()
        {
            foreach (var element in _elements)
            {
                element.Initialize();
            }
            Controller.Bind(this);
        }

        public virtual void Refresh()
        {
            foreach (var element in _elements)
            {
                if (element is IDynamic dynamic)
                {
                    dynamic.Refresh();
                }
            }
        }

        public virtual void ResizeContext(Vector3 bounds)
        {
            foreach (var element in _elements)
            {
                element.ResizeContext(bounds);
            }
        }

        public virtual void Update(long delta)
        {
            foreach (var element in _elements)
            {
                element.Update(delta);
            }
        }

        protected override void DisposeImpl()
        {
            Controller.Unbind();
            foreach (var element in _elements)
            {
                if (element is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }
    }
}
