using Cardamom.Graphics;
using Cardamom.Ui;
using Expeditionary.Hexagons;
using Expeditionary.Model.Knowledge;
using Expeditionary.Model.Mapping;
using OpenTK.Mathematics;

namespace Expeditionary.View
{
    public class FogOfWarLayer : GraphicsResource, IRenderable
    {
        private static readonly Color4 s_Undiscovered = new(0, 0, 0, 1f);
        private static readonly Color4 s_Hidden = new(0, 0, 0, 0.5f);
        private static readonly Color4 s_Visible = new(0, 0, 0, 0);

        private VertexBuffer<Vertex3>? _buffer;
        private readonly Vertex3[] _vertices;
        private readonly RenderShader _shader;
        private readonly Texture _texture;

        internal FogOfWarLayer(
            VertexBuffer<Vertex3> buffer, Vertex3[] vertices, RenderShader shader, Texture texture)
        {
            _buffer = buffer;
            _vertices = vertices;
            _shader = shader;
            _texture = texture;
        }

        public void SetAll(Map map, MapKnowledge knowledge)
        {
            foreach (var hex in map.GetTiles())
            {
                var tile = map.GetTile(hex);
                foreach ((var corner, var index) in Geometry.GetCorners(hex))
                {
                    var triangle = 3 * index + 9 * GetIndex(corner, map.Height);
                    var color = GetColor(knowledge.Get(hex));
                    _vertices[triangle].Color = color;
                    _vertices[triangle + 1].Color = color;
                    _vertices[triangle + 2].Color = color;
                }
            }
            _buffer!.Buffer(_vertices);
        }

        public void Draw(IRenderTarget target, IUiContext context)
        {
            target.Draw(_buffer!, 0, _buffer!.Length, new(BlendMode.Alpha, _shader, _texture));
        }

        public void Initialize() { }

        public void ResizeContext(Vector3 bounds) { }

        public void Update(long delta) { }

        protected override void DisposeImpl()
        {
            _buffer?.Dispose();
            _buffer = null;
        }

        private static Color4 GetColor(TileKnowledge knowledge)
        {
            if (!knowledge.IsDiscovered)
            {
                return s_Undiscovered;
            }
            if (!knowledge.IsVisible)
            {
                return s_Hidden;
            }
            return s_Visible;
        }

        private static int GetIndex(Vector3i tri, int height)
        {
            var coord = Cubic.TriangularOffset.Instance.Project(tri);
            return coord.Y + coord.X * (2 * height + 1);
        }
    }
}
