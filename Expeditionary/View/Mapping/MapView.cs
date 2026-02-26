using Cardamom;
using Cardamom.Graphics;
using Cardamom.Ui;
using Expeditionary.View.Common.Buffers;
using Expeditionary.View.Textures;
using OpenTK.Mathematics;

namespace Expeditionary.View.Mapping
{
    public class MapView : ManagedResource, IRenderable
    {
        private VertexBuffer<Vertex3>? _grid;
        private LayeredVertexBuffer? _terrain;
        private readonly RenderShader _maskShader;
        private readonly RenderShader _gridShader;
        private readonly MapTextureLibrary _textureLibrary;

        private Color4 _gridFilter = Color4.White;

        internal MapView(
            VertexBuffer<Vertex3>? grid,
            LayeredVertexBuffer? terrain,
            RenderShader maskShader,
            RenderShader gridShader,
            MapTextureLibrary textureLibrary)
        {
            _grid = grid;
            _terrain = terrain;
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
        }

        public void Draw(IRenderTarget target, IUiContext context)
        {
            _terrain!.GetLayer(0).Draw(target, GetRenderResources());
            _terrain!.GetLayer(1).Draw(target, GetFoliageRenderResources());
            _terrain!.GetLayer(2).Draw(target, GetRenderResources());
            _terrain!.GetLayer(3).Draw(target, GetRidgeRenderResources());
            _terrain!.GetLayer(4).Draw(target, GetRiverRenderResources());
            _terrain!.GetLayer(5).Draw(target, GetStructureRenderResources());

            _gridShader.SetColor("filter_color", _gridFilter);
            target.Draw(
                _grid!,
                0,
                _grid!.Length,
                new RenderResources(BlendMode.Alpha, _gridShader));
        }

        public void Initialize() { }

        public void ResizeContext(Vector3 size) { }

        public void SetGridAlpha(float alpha)
        {
            _gridFilter.A = alpha;
        }

        public void Update(long delta) { }

        private RenderResources GetFoliageRenderResources()
        {
            return new(
                BlendMode.Alpha, _maskShader, _textureLibrary.Partitions.Texture, _textureLibrary.Foliage.Texture);
        }

        private RenderResources GetRenderResources()
        {
            return new(BlendMode.Alpha, _maskShader, _textureLibrary.Partitions.Texture, _textureLibrary.Blank);
        }

        private RenderResources GetRiverRenderResources()
        {
            return new(BlendMode.Alpha, _maskShader, _textureLibrary.Rivers.Texture, _textureLibrary.Blank);
        }

        private RenderResources GetRidgeRenderResources()
        {
            return new(BlendMode.Alpha, _maskShader, _textureLibrary.Ridges.Texture, _textureLibrary.Blank);
        }

        private RenderResources GetStructureRenderResources()
        {
            return new(BlendMode.Alpha, _maskShader, _textureLibrary.Structures.Texture, _textureLibrary.Blank);
        }
    }
}
