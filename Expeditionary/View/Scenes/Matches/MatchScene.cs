using Cardamom.Graphics;
using Cardamom.Graphics.Camera;
using Cardamom.Mathematics.Geometry;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using OpenTK.Mathematics;

namespace Expeditionary.View.Scenes.Matches
{
    public class MatchScene : GraphicsResource, IScene
    {
        public IElementController Controller { get; }
        public IControlledElement? Parent { get; set; }
        public ICamera Camera { get; }
        public float? OverrideDepth { get; set; }

        public InteractiveModel Map { get; }
        public InteractiveModel Assets { get; }
        public HighlightLayer Highlight { get; }

        public MatchScene(
            IElementController controller,
            ICamera camera,
            InteractiveModel map,
            InteractiveModel assets,
            HighlightLayer highlight)
        {
            Controller = controller;
            Camera = camera;
            Map = map;
            Assets = assets;
            Highlight = highlight;
        }

        protected override void DisposeImpl()
        {
            Map.Dispose();
            Assets.Dispose();
            Highlight.Dispose();
        }

        public void Draw(IRenderTarget target, IUiContext context)
        {
            target.PushViewMatrix(Camera.GetViewMatrix());
            target.PushProjection(Camera.GetProjection());
            context.Register(this);
            Map.Draw(target, context);
            Highlight.Draw(target, context);
            Assets.Draw(target, context);
            target.PopProjectionMatrix();
            target.PopViewMatrix();
        }

        public float? GetRayIntersection(Ray3 ray)
        {
            return float.MaxValue;
        }

        public void Initialize()
        {
            Map.Initialize();
            Assets.Initialize();
            Highlight.Initialize();
            Controller.Bind(this);
        }

        public void ResizeContext(Vector3 bounds)
        {
            Camera.SetAspectRatio(bounds.X / bounds.Y);
            Map.ResizeContext(bounds);
        }

        public void Update(long delta)
        {
            Map.Update(delta);
            Assets.Update(delta);
            Highlight.Update(delta);
        }
    }
}
