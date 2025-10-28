using Cardamom;
using Cardamom.Collections;
using Cardamom.Graphics;
using Cardamom.Graphics.TexturePacking;
using Cardamom.Ui;
using Expeditionary.Hexagons;
using Expeditionary.Model;
using Expeditionary.Model.Knowledge;
using Expeditionary.Model.Units;
using Expeditionary.View.Common.Buffers;
using OpenTK.Mathematics;

namespace Expeditionary.View.Scenes.Matches.Layers
{
    public class AssetLayer : ManagedResource, IRenderable
    {
        private class AssetComparator : IComparer<(IAsset, SingleAssetKnowledge)>
        {
            public int Compare((IAsset, SingleAssetKnowledge) left, (IAsset, SingleAssetKnowledge) right)
            {
                (var leftAsset, var leftKnowledge) = left;
                (var rightAsset, var rightKnowledge) = right;
                if (leftAsset is Unit leftUnit && leftUnit.Passenger == rightAsset)
                {
                    return 1;
                }
                if (rightAsset is Unit rightUnit && rightUnit.Passenger == leftAsset)
                {
                    return -1;
                }
                var visible = leftKnowledge.IsVisible.CompareTo(rightKnowledge.IsVisible);
                if (visible == 0)
                {
                    var points = leftAsset.Value.Points.CompareTo(rightAsset.Value.Points);
                    if (points == 0)
                    {
                        return leftAsset.Id.CompareTo(rightAsset.Id);
                    }
                    return points;
                }
                return visible;
            }
        }

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
        private readonly Dictionary<Vector3i, int> _addressMap = new();
        private readonly Dictionary<IAsset, Vector3i> _positionMap = new();
        private readonly MultiMap<Vector3i, IAsset> _assetMap = new();

        private IPlayerKnowledge? _knowledge;

        public AssetLayer(RenderShader shader, ITextureVolume textures)
        {
            _shader = shader;
            _textures = textures;
            _vertices = new(512, 12, GetRenderResources);
        }

        public void Draw(IRenderTarget target, IUiContext context)
        {
            _vertices!.Draw(target, context);
        }

        public IEnumerable<IAsset> GetAssetsAt(Vector3i hex)
        {
            if (_assetMap.TryGetValue(hex, out var assets))
            {
                return assets
                    .Select(x => (x, _knowledge!.GetAsset(x)))
                    .Order(new AssetComparator())
                    .Reverse()
                    .Select(x => x.Item1);
            }
            return Enumerable.Empty<IAsset>();
        }

        public void Initialize()
        {
            _vertices!.Initialize();
        }

        public void SetAll(Match match, IPlayerKnowledge knowledge)
        {
            _knowledge = knowledge;
            _addressMap.Clear();
            _positionMap.Clear();
            _assetMap.Clear();
            _vertices!.Clear();
            foreach (var asset in match.GetAssets())
            {
                var k = _knowledge.GetAsset(asset);
                if (k.IsVisible || k.LastSeen != null)
                {
                    _positionMap.Add(asset, k.LastSeen!.Value);
                    _assetMap.Add(k.LastSeen!.Value, asset);
                }
            }
            foreach (var hex in _assetMap.Keys)
            {
                Update(hex);
            }
        }

        public void Set(IEnumerable<IAsset> delta)
        {
            foreach (var asset in delta)
            {
                var k = _knowledge!.GetAsset(asset);
                if (k.IsVisible || k.LastSeen != null)
                {
                    Place(asset, k.LastSeen!.Value);
                }
                else
                {
                    Remove(asset);
                }
            }
        }

        public void Remove(IAsset asset)
        {
            if (_positionMap.TryGetValue(asset, out var lastPosition))
            {
                _assetMap.Remove(lastPosition, asset);
                Update(lastPosition);
            }
            _positionMap.Remove(asset);
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

        private int Add(Vector3i hex)
        {
            var block = _vertices!.Reserve();
            _addressMap.Add(hex, block);
            return block;
        }

        private int GetOrAdd(Vector3i hex)
        {
            if (_addressMap.TryGetValue(hex, out var block))
            {
                return block;
            }
            return Add(hex);
        }

        private void Place(IAsset asset, Vector3i position)
        {
            if (_positionMap.TryGetValue(asset, out var lastPosition))
            {
                _assetMap.Remove(lastPosition, asset);
                Update(lastPosition);
            }
            _positionMap[asset] = position;
            _assetMap.Add(position, asset);
            Update(position);
        }

        private void Return(Vector3i hex)
        {
            if (_addressMap.TryGetValue(hex, out var block))
            {
                _vertices!.Free(block);
            }
            _addressMap.Remove(hex);
        }

        private void Update(Vector3i hex)
        {
            if (_assetMap.TryGetValue(hex, out var assets))
            {
                var block = GetOrAdd(hex);
                var vertices = new Vertex3[12];
                var p = ToVector3(Cubic.Cartesian.Instance.Project(hex));
                (var asset, var k) = GetTopAsset(assets, _knowledge!);
                var filter = k.IsVisible ? Color4.White : s_Invisible;
                SetVertices(
                    vertices, 0, p, Combine(filter, GetBackground(asset)), _textures.Get(s_BackgroundKey).TextureView);
                SetVertices(
                    vertices, 6, p, Combine(filter, GetForeground(asset)), _textures.Get(asset.TypeKey).TextureView);
                _vertices!.Set(block, vertices);
            }
            else
            {
                Return(hex);
            }
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

        private static (IAsset, SingleAssetKnowledge) GetTopAsset(
            IEnumerable<IAsset> assets, IPlayerKnowledge knowledge)
        {
            if (!assets.Any())
            {
                throw new IndexOutOfRangeException();
            }
            if (assets.Count() == 1)
            {
                var asset = assets.First();
                return (asset, knowledge.GetAsset(asset));
            }
            return assets.Select(x => (x, knowledge.GetAsset(x))).Max(new AssetComparator());
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
