using Cardamom.Graphics;
using Cardamom.Ui;
using Expeditionary.View.Common.Buffers;
using OpenTK.Mathematics;

namespace Expeditionary.View.Mapping
{
    public class MapView : GraphicsResource, IRenderable
    {
        private VertexBuffer<Vertex3>? _grid;
        private LayeredVertexBuffer? _terrain;

        private readonly RenderShader _gridShader;
        private Color4 _gridFilter = Color4.White;

        private RenderTexture? _maskTexture;

        internal MapView(
            VertexBuffer<Vertex3>? grid,
            LayeredVertexBuffer? terrain,
            RenderShader maskShader)
        {
            _grid = grid;
            _terrain = terrain;
            _gridShader = maskShader;
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

            _gridShader.SetColor("filter_color", _gridFilter);
            target.Draw(
                _grid!,
                0,
                _grid!.Length,
                new RenderResources(BlendMode.Alpha, _gridShader, _maskTexture!.GetTexture()));
        }

        public void Initialize() { }

        public void ResizeContext(Vector3 size)
        {
            _maskTexture?.Dispose();
            _maskTexture = new RenderTexture(new((int)size.X, (int)size.Y));
        }

        public void SetGridAlpha(float alpha)
        {
            _gridFilter.A = alpha;
        }

        public void Update(long delta) { }
    }
}
