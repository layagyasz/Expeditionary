using Cardamom.Graphics;
using Cardamom.Graphics.TexturePacking;
using Cardamom.Ui;
using Expeditionary.Hexagons;
using Expeditionary.Model;
using Expeditionary.Model.Combat;
using Expeditionary.Model.Combat.Units;
using Expeditionary.Model.Knowledge;
using Expeditionary.View.Common.Buffers;
using OpenTK.Mathematics;

namespace Expeditionary.View
{
    public class AssetLayer : GraphicsResource, IRenderable
    {
        private static readonly Color4 s_Invisible = new(2, 2, 2, 0.25f);
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

        public void Add(IAsset asset, SingleAssetKnowledge knowledge)
        {
            int block = _vertices!.Reserve();
            _addressMap.Add(asset.Id, block);
            Place(asset, knowledge.LastSeen!.Value, block, knowledge.IsVisible ? Color4.White : s_Invisible);
        }

        public void Draw(IRenderTarget target, IUiContext context)
        {
            _vertices!.Draw(target, context);
        }

        public void Initialize()
        {
            _vertices!.Initialize();
        }

        public void SetAll(Match match, PlayerKnowledge knowledge)
        {
            _addressMap.Clear();
            _vertices!.Clear();
            foreach (var asset in match.GetAssets())
            {
                var k = knowledge.GetAsset(asset);
                if (k.IsVisible || k.LastSeen != null)
                {
                    Add(asset, k);
                }
            }
        }

        public void Set(PlayerKnowledge knowledge, IEnumerable<IAsset> delta)
        {
            foreach (var asset in delta)
            {
                var k = knowledge.GetAsset(asset);
                Console.WriteLine($"{asset.Id} {k}");
                if (k.IsVisible || k.LastSeen != null)
                {
                    if (!_addressMap.ContainsKey(asset.Id))
                    {
                        Add(asset, k);
                    } 
                    else
                    {
                        Place(
                            asset, k.LastSeen!.Value, _addressMap[asset.Id], k.IsVisible ? Color4.White : s_Invisible);
                    }
                }
                else
                {
                    Remove(asset);
                }
            }
        }

        public void Remove(IAsset asset)
        {
            if (_addressMap.TryGetValue(asset.Id, out var block))
            {
                _addressMap.Remove(asset.Id);
                _vertices!.Set(block, new Vertex3[12]);
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

        private void Place(IAsset asset, Vector3i position, int block, Color4 filter)
        {
            var vertices = new Vertex3[12];
            var p = ToVector3(Cubic.Cartesian.Instance.Project(position));
            SetVertices(
                vertices, 0, p, Combine(filter, GetBackground(asset)), _textures.Get(s_BackgroundKey).TextureView);
            SetVertices(
                vertices, 6, p, Combine(filter, GetForeground(asset)), _textures.Get(asset.TypeKey).TextureView);
            _vertices!.Set(block, vertices);
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

        private static Color4 Combine(Color4 left, Color4 right)
        {
            return (Color4)((Vector4)left * (Vector4)right);
        }

        private static Vector3 ToVector3(Vector2 x)
        {
            return new(x.X, 0, x.Y);
        }
    }
}
