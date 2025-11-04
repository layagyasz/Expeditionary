using Cardamom.Ui;
using Cardamom;
using Cardamom.Graphics.Camera;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using Cardamom.Graphics;
using Cardamom.Mathematics.Geometry;
using OpenTK.Mathematics;

namespace Expeditionary.View.Scenes.Galaxies
{
    public class GalaxyScene : ManagedResource, IScene
    {
        public IElementController Controller { get; }
        public IControlledElement? Parent { get; set; }
        public ICamera Camera { get; }
        public float? OverrideDepth { get; set; }

        public InteractiveModel Galaxy { get; }

        public GalaxyScene(
            IElementController controller,
            ICamera camera,
            InteractiveModel galaxy)
        {
            Controller = controller;
            Camera = camera;
            Galaxy = galaxy;
        }

        protected override void DisposeImpl()
        {
            Galaxy.Dispose();
        }

        public void Draw(IRenderTarget target, IUiContext context)
        {
            target.PushViewMatrix(Camera.GetViewMatrix());
            target.PushProjection(Camera.GetProjection());
            context.Register(this);
            Galaxy.Draw(target, context);
            target.PopProjectionMatrix();
            target.PopViewMatrix();
        }

        public float? GetRayIntersection(Ray3 ray)
        {
            return float.MaxValue;
        }

        public void Initialize()
        {
            Galaxy.Initialize();
            Controller.Bind(this);
        }

        public void ResizeContext(Vector3 bounds)
        {
            Camera.SetAspectRatio(bounds.X / bounds.Y);
            Galaxy.ResizeContext(bounds);
        }

        public void Update(long delta)
        {
            Galaxy.Update(delta);
        }
    }
}
