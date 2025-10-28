using Cardamom;
using Cardamom.Graphics;
using Cardamom.Mathematics;
using Cardamom.Ui;
using Expeditionary.Evaluation.Considerations;
using Expeditionary.Hexagons;
using Expeditionary.Model.Mapping;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Expeditionary.View.Scenes.Matches.Layers
{
    public class HighlightLayer : ManagedResource, IRenderable
    {
        public record struct HexHighlight(Vector3i Hex, int Level);

        private static readonly float s_Sqrt3d2 = 0.5f * MathF.Sqrt(3);
        private static readonly Vector3[] s_Corners =
        {
            new(-0.5f, 0, -s_Sqrt3d2),
            new(0.5f, 0, -s_Sqrt3d2),
            new(1, 0, 0),
            new(0.5f, 0, s_Sqrt3d2),
            new(-0.5f, 0, s_Sqrt3d2),
            new(-1, 0, 0)
        };
        private static readonly float s_Scale = 0.5f;

        private static readonly Color4[] s_Levels =
        {
            new(0f, 1f, 0f, 0.5f),
            new(1f, 1f, 0f, 0.5f),
            new(1f, 0.5f, 0f, 0.5f),
            new(1f, 0f, 0f, 0.5f)
        };
        public static readonly int Levels = s_Levels.Length;

        private readonly RenderShader _shader;

        private VertexBuffer<Vertex3>? _vertices;

        public HighlightLayer(RenderShader shader)
        {
            _shader = shader;
            _vertices = new(PrimitiveType.Triangles);
        }

        public void Draw(IRenderTarget target, IUiContext context)
        {
            target.Draw(_vertices!, 0, _vertices!.Length, new(BlendMode.Alpha, _shader));
        }

        public void Initialize()
        {
            _vertices!.Buffer(Array.Empty<Vertex3>());
        }

        public void ResizeContext(Vector3 context) { }

        public void SetHighlight(IEnumerable<HexHighlight> highlight)
        {
            var hexes = highlight.ToArray();
            var vertices = new Vertex3[18 * hexes.Length];
            int v = 0;
            for (int i = 0; i < hexes.Length; ++i)
            {
                var hex = hexes[i];
                var color = s_Levels[hex.Level];
                var center = Cubic.Cartesian.Instance.Project(hex.Hex);
                for (int j = 0; j < 6; ++j)
                {
                    vertices[v++] = new(ToVector3(center), color, new());
                    vertices[v++] = new(ToVector3(center) + s_Scale * s_Corners[j], color, new());
                    vertices[v++] = new(ToVector3(center) + s_Scale * s_Corners[(j + 1) % 6], color, new());
                }
            }
            _vertices!.Buffer(vertices);
        }

        public void Update(long delta) { }

        protected override void DisposeImpl()
        {
            _vertices?.Dispose();
            _vertices = null;
        }

        public static IEnumerable<HexHighlight> ForConsideration(Map map, TileConsideration consideration)
        {
            return map.Range()
                .Select(x => new HexHighlight(
                    x, GetUnitIntervalLevel(TileConsiderations.Evaluate(consideration, x, map))));
        }

        public static int GetUnitIntervalLevel(float value)
        {
            if (value <= float.Epsilon)
            {
                return 0;
            }
            if (value >= 1)
            {
                return Levels - 1;
            }
            return (int)(Levels * value);
        }

        public static int GetLevel(float value, Interval interval)
        {
            if (value <= interval.Minimum)
            {
                return 0;
            }
            if (value >= interval.Maximum)
            {
                return Levels - 1;
            }
            return (int)(Levels * (value - interval.Minimum) / (interval.Maximum - interval.Minimum));
        }

        private static Vector3 ToVector3(Vector2 x)
        {
            return new(x.X, 0, x.Y);
        }
    }
}
