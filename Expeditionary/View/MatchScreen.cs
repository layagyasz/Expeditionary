using Cardamom.Graphics;
using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Expeditionary.View.Common;
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
        public RightClickMenu? UnitSelect { get; private set; }

        public MatchScreen(
            IController controller, MatchScene scene, UnitOverlay unitOverlay, RightClickMenu unitSelect)
        {
            Controller = controller;
            Scene = scene;
            UnitOverlay = unitOverlay;
            UnitSelect = unitSelect;
        }

        public void Draw(IRenderTarget target, IUiContext context)
        {
            Scene!.Draw(target, context);
            context.Flatten();
            UnitOverlay!.Draw(target, context);
            UnitSelect!.Draw(target, context);
        }

        public void Initialize()
        {
            Scene!.Initialize();
            UnitOverlay!.Initialize();
            UnitSelect!.Initialize();
            Controller.Bind(this);
        }

        public void ResizeContext(Vector3 bounds)
        {
            Scene!.ResizeContext(bounds);
            UnitOverlay!.ResizeContext(bounds);
            UnitSelect!.ResizeContext(bounds);
        }

        public void Update(long delta)
        {
            Updated?.Invoke(this, EventArgs.Empty);
            Scene!.Update(delta);
            UnitOverlay!.Update(delta);
            UnitSelect!.Update(delta);
        }

        protected override void DisposeImpl()
        {
            Scene!.Dispose();
            Scene = null;
            UnitOverlay!.Dispose();
            UnitOverlay = null;
            UnitSelect!.Dispose();
            UnitSelect = null;
        }
    }
}
