using Cardamom.Graphics;
using Cardamom.Ui;
using OpenTK.Mathematics;

namespace Expeditionary.View
{
    public class MapView : GraphicsResource, IRenderable
    {
        private VertexBuffer<Vertex3>? _grid;
        private LayeredVertexBuffer? _terrain;
        private readonly RenderShader _maskShader;

        private RenderTexture? _maskTexture;

        internal MapView(
            VertexBuffer<Vertex3>? grid,
            LayeredVertexBuffer? terrain,
            RenderShader maskShader)
        {
            _grid = grid;
            _terrain = terrain;
            _maskShader = maskShader;
        }

        protected override void DisposeImpl()
        {
            _grid?.Dispose();
            _grid = null;

            _terrain?.Dispose();
            _terrain = null;
        }

        public void Draw(IRenderTarget target, IUiContext context) 
        {
            _terrain!.Draw(target, context);
            target.Draw(
                _grid!,
                0, 
                _grid!.Length,
                new RenderResources(BlendMode.Alpha, _maskShader, _maskTexture!.GetTexture()));
        }

        public void Initialize() { }

        public void ResizeContext(Vector3 size)
        {
            _maskTexture?.Dispose();
            _maskTexture = new RenderTexture(new((int)size.X, (int)size.Y));
        }

        public void SetGridAlpha(float alpha)
        {
            _maskTexture?.Clear(new(1, 1, 1, alpha));
        }

        public void Update(long delta) { }
    }
}
