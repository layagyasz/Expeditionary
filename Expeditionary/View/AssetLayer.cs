using Cardamom.Graphics;
using Cardamom.Graphics.TexturePacking;
using Cardamom.Ui;
using Expeditionary.Hexagons;
using Expeditionary.Model.Combat;
using Expeditionary.Model.Combat.Units;
using Expeditionary.View.Common.Buffers;
using OpenTK.Mathematics;

namespace Expeditionary.View
{
    public class AssetLayer : GraphicsResource, IRenderable
    {
        private static readonly string s_BackgroundKey = "icon_unit_background";
        private static readonly float s_Sqrt3_2 = 0.5f * MathF.Sqrt(3);
        private static readonly Vector3[] s_Corners =
        {
            new(-s_Sqrt3_2, 0f, -s_Sqrt3_2),
            new(s_Sqrt3_2, 0f, -s_Sqrt3_2),
            new(-s_Sqrt3_2, 0f, s_Sqrt3_2),
            new(-s_Sqrt3_2, 0f, s_Sqrt3_2),
            new(s_Sqrt3_2, 0f, -s_Sqrt3_2),
            new(s_Sqrt3_2, 0f, s_Sqrt3_2)
        };

        private readonly RenderShader _shader;
        private readonly ITextureVolume _textures;

        private SegmentedVertexBuffer<Vertex3>? _vertices;
        private readonly Dictionary<int, int> _addressMap = new();

        public AssetLayer(RenderShader shader, ITextureVolume textures)
        {
            _shader = shader;
            _textures = textures;
            _vertices = new(512, 12, GetRenderResources);
        }

        public void Add(IAsset asset)
        {
            int block = _vertices!.Reserve();
            _addressMap.Add(asset.Id, block);

            var vertices = new Vertex3[12];
            var position = ToVector3(Cubic.Cartesian.Instance.Project(asset.Position));
            SetVertices(vertices, 0, position, GetBackground(asset), _textures.Get(s_BackgroundKey).TextureView);
            SetVertices(vertices, 6, position, GetForeground(asset), _textures.Get(asset.TypeKey).TextureView);
            _vertices!.Set(block, vertices);
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

        private static Color4 GetBackground(IAsset asset)
        {
            if (asset is Unit unit)
            {
                return unit.Player.Faction.ColorScheme.Background;
            }
            return Color4.White;
        }

        private static Color4 GetForeground(IAsset asset)
        {
            if (asset is Unit unit)
            {
                return unit.Player.Faction.ColorScheme.Foreground;
            }
            return Color4.Black;
        }

        private static void SetVertices(
            Vertex3[] vertices, int offset, Vector3 position, Color4 color, Box2i textureView)
        {
            vertices[offset] = new(position + s_Corners[0], color, textureView.Min);
            vertices[offset + 1] = new(position + s_Corners[1], color, new(textureView.Max.X, textureView.Min.Y));
            vertices[offset + 2] = new(position + s_Corners[2], color, new(textureView.Min.X, textureView.Max.Y));
            vertices[offset + 3] = new(position + s_Corners[3], color, new(textureView.Min.X, textureView.Max.Y));
            vertices[offset + 4] = new(position + s_Corners[4], color, new(textureView.Max.X, textureView.Min.Y));
            vertices[offset + 5] = new(position + s_Corners[5], color, textureView.Max);
        }

        private static Vector3 ToVector3(Vector2 x)
        {
            return new(x.X, 0, x.Y);
        }
    }
}
