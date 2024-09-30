using Cardamom.Graphics;
using Cardamom.Graphics.TexturePacking;
using Cardamom.Ui;
using Expeditionary.Hexagons;
using Expeditionary.Model.Combat;
using Expeditionary.View.Common.Buffers;
using OpenTK.Mathematics;

namespace Expeditionary.View
{
    public class AssetLayer: GraphicsResource, IRenderable
    {
        private static readonly Vector3[] s_Corners =
        {
            new(-0.5f, 0f, -0.5f),
            new(0.5f, 0f, -0.5f),
            new(-0.5f, 0f, 0.5f),
            new(-0.5f, 0f, 0.5f),
            new(0.5f, 0f, -0.5f),
            new(0.5f, 0f, 0.5f)
        };

        private readonly RenderShader _shader;
        private readonly ITextureVolume _textures;

        private SegmentedVertexBuffer<Vertex3>? _vertices;
        private readonly Dictionary<int, int> _addressMap = new();

        public AssetLayer(RenderShader shader, ITextureVolume textures)
        {
            _shader = shader;
            _textures = textures;
            _vertices = new(512, 6, GetRenderResources);
        }

        public void Add(IAsset asset)
        {
            int block = _vertices!.Reserve();
            _addressMap.Add(asset.Id, block);
            _vertices!.Set(block, GetVertices(asset.Position, _textures.Get(asset.TypeKey).TextureView));
        }

        public void Draw(IRenderTarget target, IUiContext context)
        {
            _vertices!.Draw(target, context);
        }

        public void Initialize()
        {
            _vertices!.Initialize();
        }

        public void Remove(IAsset asset)
        {
            if (_addressMap.TryGetValue(asset.Id, out var block))
            {
                _addressMap.Remove(asset.Id);
                _vertices!.Free(block);
            }
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

        private static Vertex3[] GetVertices(Vector3i hex, Box2i textureView)
        {
            Vector3 position = ToVector3(Geometry.MapCubic(hex));
            var color = Color4.White;
            return new Vertex3[]
            {
                new(position + s_Corners[0], color, textureView.Min),
                new(position + s_Corners[1], color, new(textureView.Max.X, textureView.Min.Y)),
                new(position + s_Corners[2], color, new(textureView.Min.X, textureView.Max.Y)),
                new(position + s_Corners[3], color, new(textureView.Min.X, textureView.Max.Y)),
                new(position + s_Corners[4], color, new(textureView.Max.X, textureView.Min.Y)),
                new(position + s_Corners[5], color, textureView.Max)
            };
        }

        private static Vector3 ToVector3(Vector2 x)
        {
            return new(x.X, 0, x.Y);
        }
    }
}
