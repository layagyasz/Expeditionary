using Cardamom.Graphics;
using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Expeditionary.View.Scenes.Matches;
using OpenTK.Mathematics;

namespace Expeditionary.View
{
    public class MatchScreen : GraphicsResource, IRenderable
    {
        public event EventHandler<EventArgs>? Updated;

        public IController Controller { get; }
        public MatchScene? Scene { get; private set; }
        public UnitOverlay? UnitOverlay { get; private set; }

        public MatchScreen(IController controller, MatchScene scene, UnitOverlay unitOverlay)
        {
            Controller = controller;
            Scene = scene;
            UnitOverlay = unitOverlay;
        }

        public void Draw(IRenderTarget target, IUiContext context)
        {
            Scene!.Draw(target, context);
            context.Flatten();
            UnitOverlay!.Draw(target, context);
        }

        public void Initialize()
        {
            Scene!.Initialize();
            UnitOverlay!.Initialize();
            Controller.Bind(this);
        }

        public void ResizeContext(Vector3 bounds)
        {
            Scene!.ResizeContext(bounds);
            UnitOverlay!.ResizeContext(bounds);
        }

        public void Update(long delta)
        {
            Updated?.Invoke(this, EventArgs.Empty);
            Scene!.Update(delta);
            UnitOverlay!.Update(delta);
        }

        protected override void DisposeImpl()
        {
            Scene!.Dispose();
            Scene = null;
            UnitOverlay!.Dispose();
            UnitOverlay = null;
        }
    }
}
