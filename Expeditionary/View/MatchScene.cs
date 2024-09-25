using Cardamom.Graphics;
using Cardamom.Graphics.Camera;
using Cardamom.Mathematics.Geometry;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using OpenTK.Mathematics;

namespace Expeditionary.View
{
    public class MatchScene : GraphicsResource, IScene
    {
        public IElementController Controller { get; }
        public IControlledElement? Parent { get; set; }
        public ICamera Camera { get; }
        public float? OverrideDepth { get; set; }

        private readonly MapView _map;
        private readonly AssetLayer _assets;

        public MatchScene(
            IElementController controller,
            ICamera camera,
            MapView map,
            AssetLayer assets) 
        {
            Controller = controller;
            Camera = camera;
            _map = map;
            _assets = assets;

            Camera.Changed += HandleCameraChanged;
        }

        protected override void DisposeImpl()
        {
            Camera.Changed -= HandleCameraChanged;
            _map.Dispose();
        }

        public void Draw(IRenderTarget target, IUiContext context)
        {
            target.PushViewMatrix(Camera.GetViewMatrix());
            target.PushProjection(Camera.GetProjection());
            context.Register(this);
            _map.Draw(target, context);
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
            Controller.Bind(this);
            _map.Initialize();
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
        }

        private void HandleCameraChanged(object? sender, EventArgs e)
        {
            _map.SetGridAlpha(GetGridAlpha(Camera.Position.Y));
        }

        private static float GetGridAlpha(float distance)
        {
            if (distance < 10)
            {
                return 1;
            }
            if (distance > 70)
            {
                return 0;
            }
            return .017f * (60 - distance);
        }
    }
}
