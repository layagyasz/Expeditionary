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

        private readonly InteractiveModel _map;
        private readonly InteractiveModel _assets;
        private readonly HighlightLayer _highlight;

        public MatchScene(
            IElementController controller,
            ICamera camera,
            InteractiveModel map,
            InteractiveModel assets,
            HighlightLayer highlight)
        {
            Controller = controller;
            Camera = camera;
            _map = map;
            _assets = assets;
            _highlight = highlight;
        }

        protected override void DisposeImpl()
        {
            _map.Dispose();
            _assets.Dispose();
            _highlight.Dispose();
        }

        public void Draw(IRenderTarget target, IUiContext context)
        {
            target.PushViewMatrix(Camera.GetViewMatrix());
            target.PushProjection(Camera.GetProjection());
            context.Register(this);
            _map.Draw(target, context);
            _highlight.Draw(target, context);
            _assets.Draw(target, context);
            target.PopProjectionMatrix();
            target.PopViewMatrix();
        }

        public float? GetRayIntersection(Ray3 ray)
        {
            return float.MaxValue;
        }

        public void Initialize()
        {
            _map.Initialize();
            _assets.Initialize();
            _highlight.Initialize();
            Controller.Bind(this);
        }

        public void ResizeContext(Vector3 bounds)
        {
            Camera.SetAspectRatio(bounds.X / bounds.Y);
            _map.ResizeContext(bounds);
        }

        public void Update(long delta)
        {
            _map.Update(delta);
            _assets.Update(delta);
            _highlight.Update(delta);
        }
    }
}
