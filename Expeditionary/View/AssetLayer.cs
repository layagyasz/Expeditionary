using Cardamom.Graphics;
using Cardamom.Graphics.TexturePacking;
using Cardamom.Ui;
using Expeditionary.View.Common.Buffers;
using OpenTK.Mathematics;

namespace Expeditionary.View
{
    public class AssetLayer: GraphicsResource, IRenderable
    {
        private readonly RenderShader _shader;
        private readonly ITextureVolume _textures;

        private SegmentedVertexBuffer<Vertex3>? _vertices;

        public AssetLayer(RenderShader shader, ITextureVolume textures)
        {
            _shader = shader;
            _textures = textures;
            _vertices = new(512, 6, GetRenderResources);
        }

        public void Draw(IRenderTarget target, IUiContext context)
        {
            _vertices!.Draw(target, context);
        }

        public void Initialize()
        {
            _vertices!.Initialize();
        }

        public void ResizeContext(Vector3 context) { }

        public void Update(long delta)
        {
            _vertices!.Update(delta);
        }

        protected override void DisposeImpl()
        {
            _vertices?.Dispose();
            _vertices = null;
        }

        private RenderResources GetRenderResources()
        {
            return new(BlendMode.Alpha, _shader, _textures.GetTextures().First());
        }
    }
}
