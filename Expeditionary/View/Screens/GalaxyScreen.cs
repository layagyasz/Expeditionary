using Cardamom;
using Cardamom.Graphics;
using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Expeditionary.View.Scenes.Galaxies;
using OpenTK.Mathematics;

namespace Expeditionary.View.Screens
{
    public class GalaxyScreen : ManagedResource, IRenderable
    {
        public IController Controller { get; }
        public GalaxyScene? Scene { get; private set; }

        public GalaxyScreen(IController controller, GalaxyScene scene)
        {
            Controller = controller;
            Scene = scene;
        }
        public void Draw(IRenderTarget target, IUiContext context)
        {
            Scene!.Draw(target, context);
            context.Flatten();
        }

        public void Initialize()
        {
            Scene!.Initialize();
            Controller.Bind(this);
        }

        public void ResizeContext(Vector3 bounds)
        {
            Scene!.ResizeContext(bounds);
        }

        public void Update(long delta)
        {
            Scene!.Update(delta);
        }

        protected override void DisposeImpl()
        {
            Controller.Unbind();
            Scene!.Dispose();
            Scene = null;
        }

    }
}
