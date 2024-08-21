using Cardamom.Graphics;
using Cardamom.Ui;
using OpenTK.Mathematics;

namespace Expeditionary.View
{
    public class MapView : GraphicsResource, IRenderable
    {
        private VertexBuffer<Vertex3>? _tileBases;
        private readonly RenderShader _tileBaseShader;

        internal MapView(VertexBuffer<Vertex3> tileBases, RenderShader tileBaseShader)
        {
            _tileBases = tileBases;
            _tileBaseShader = tileBaseShader;
        }

        protected override void DisposeImpl()
        {
            _tileBases?.Dispose();
            _tileBases = null;
        }

        public void Draw(IRenderTarget target, IUiContext context) 
        {
            target.Draw(_tileBases!, 0, _tileBases!.Length, new RenderResources(BlendMode.None, _tileBaseShader));
        }

        public void Initialize() { }

        public void ResizeContext(Vector3 size) { }

        public void Update(long delta) { }
    }
}
