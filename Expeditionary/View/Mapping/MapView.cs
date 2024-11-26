using Cardamom.Graphics;
using Cardamom.Ui;
using Expeditionary.View.Common.Buffers;
using Expeditionary.View.Textures;
using OpenTK.Mathematics;

namespace Expeditionary.View.Mapping
{
    public class MapView : GraphicsResource, IRenderable
    {
        private VertexBuffer<Vertex3>? _grid;
        private LayeredVertexBuffer? _terrain;
        private LayeredVertexBuffer? _mask;
        private readonly RenderShader _texShader;
        private readonly RenderShader _maskShader;
        private readonly RenderShader _gridShader;
        private readonly TextureLibrary _textureLibrary;

        private Color4 _gridFilter = Color4.White;

        private RenderTexture? _maskTexture;

        internal MapView(
            VertexBuffer<Vertex3>? grid,
            LayeredVertexBuffer? terrain,
            LayeredVertexBuffer? mask,
            RenderShader texShader,
            RenderShader maskShader,
            RenderShader gridShader,
            TextureLibrary textureLibrary)
        {
            _grid = grid;
            _terrain = terrain;
            _mask = mask;
            _texShader = texShader;
            _maskShader = maskShader;
            _gridShader = gridShader;
            _textureLibrary = textureLibrary;
        }

        protected override void DisposeImpl()
        {
            _grid?.Dispose();
            _grid = null;

            _terrain?.Dispose();
            _terrain = null;

            _mask?.Dispose();
            _mask = null;
        }

        public void Draw(IRenderTarget target, IUiContext context)
        {
            _maskTexture!.PushModelMatrix(target.GetModelMatrix());
            _maskTexture.PushViewMatrix(target.GetViewMatrix());
            _maskTexture.PushProjection(target.GetProjection());
            _maskTexture.Clear();
            _mask!.GetLayer(0).Draw(_maskTexture, GetMaskRenderResources());
            _maskTexture.Display();
            _maskTexture.PopProjectionMatrix();
            _maskTexture.PopViewMatrix();
            _maskTexture.PopModelMatrix();

            _terrain!.GetLayer(0).Draw(target, GetRenderResources());
            _terrain!.GetLayer(1).Draw(target, GetFoliageRenderResources());
            _terrain!.GetLayer(2).Draw(target, GetRenderResources());
            _terrain!.GetLayer(3).Draw(target, GetRiverRenderResources());
            _terrain!.GetLayer(4).Draw(target, GetStructureRenderResources());

            _gridShader.SetColor("filter_color", _gridFilter);
            target.Draw(
                _grid!,
                0,
                _grid!.Length,
                new RenderResources(BlendMode.Alpha, _gridShader));
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

        private RenderResources GetFoliageRenderResources()
        {
            return new(
                BlendMode.Alpha, _maskShader, _textureLibrary.Partitions.GetTexture(), _maskTexture!.GetTexture());
        }

        private RenderResources GetMaskRenderResources()
        {
            return new(BlendMode.Alpha, _texShader, _textureLibrary.Masks.GetTexture());
        }

        private RenderResources GetRenderResources()
        {
            return new(BlendMode.Alpha, _texShader, _textureLibrary.Partitions.GetTexture());
        }

        private RenderResources GetRiverRenderResources()
        {
            return new(BlendMode.Alpha, _texShader, _textureLibrary.Edges.GetTexture());
        }

        private RenderResources GetStructureRenderResources()
        {
            return new(BlendMode.Alpha, _texShader, _textureLibrary.Structures.GetTexture());
        }
    }
}
