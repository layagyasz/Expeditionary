using Cardamom.Graphics;
using Cardamom.Ui;
using OpenTK.Mathematics;

namespace Expeditionary.View
{
    public class MapView : GraphicsResource, IRenderable
    {
        private VertexBuffer<Vertex3>? _grid;
        private VertexBuffer<Vertex3>? _terrain;
        private readonly Texture _terrainTexture;
        private VertexBuffer<Vertex3>? _edges;
        private readonly Texture _edgeTexture;
        private readonly RenderShader _noTexShader;
        private readonly RenderShader _texShader;

        internal MapView(
            VertexBuffer<Vertex3>? grid,
            VertexBuffer<Vertex3> terrain, 
            Texture terrainTexture,
            VertexBuffer<Vertex3> edges, 
            Texture edgeTexture,
            RenderShader noTexShader,
            RenderShader texShader)
        {
            _grid = grid;
            _terrain = terrain;
            _terrainTexture = terrainTexture;
            _edges = edges;
            _edgeTexture = edgeTexture;
            _noTexShader = noTexShader;
            _texShader = texShader;
        }

        protected override void DisposeImpl()
        {
            _grid?.Dispose();
            _grid = null;

            _terrain?.Dispose();
            _terrain = null;

            _edges?.Dispose();
            _edges = null;
        }

        public void Draw(IRenderTarget target, IUiContext context) 
        {
            target.Draw(
                _terrain!,
                0, 
                _terrain!.Length, 
                new RenderResources(BlendMode.Alpha, _texShader, _terrainTexture));
            target.Draw(
                _edges!,
                0,
                _edges!.Length,
                new RenderResources(BlendMode.Alpha, _texShader, _edgeTexture));
            target.Draw(_grid!, 0, _grid!.Length, new RenderResources(BlendMode.Alpha, _noTexShader));
        }

        public void Initialize() { }

        public void ResizeContext(Vector3 size) { }

        public void Update(long delta) { }
    }
}
