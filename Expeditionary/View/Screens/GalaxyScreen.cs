using Cardamom;
using Cardamom.Graphics;
using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Expeditionary.View.Scenes.Galaxies;
using OpenTK.Mathematics;

namespace Expeditionary.View.Screens
{
    public class GalaxyScreen : ManagedResource, IScreen
    {
        public IController Controller { get; }
        public GalaxyScene? Scene { get; private set; }
        public MissionPane? MissionPane { get; private set; }

        public GalaxyScreen(IController controller, GalaxyScene scene, MissionPane? missionPane)
        {
            Controller = controller;
            Scene = scene;
            MissionPane = missionPane;
        }
        public void Draw(IRenderTarget target, IUiContext context)
        {
            Scene!.Draw(target, context);
            context.Flatten();
            target.Flatten();
            MissionPane!.Draw(target, context);
        }

        public void Initialize()
        {
            Scene!.Initialize();
            MissionPane!.Initialize();
            Controller.Bind(this);
        }

        public void ResizeContext(Vector3 bounds)
        {
            Scene!.ResizeContext(bounds);
            MissionPane!.ResizeContext(bounds);
        }

        public void Update(long delta)
        {
            Scene!.Update(delta);
            MissionPane!.Update(delta);
        }

        protected override void DisposeImpl()
        {
            Controller.Unbind();
            Scene!.Dispose();
            Scene = null;
            MissionPane!.Dispose();
            MissionPane = null;
        }

    }
}
