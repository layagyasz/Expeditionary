using Cardamom.Graphics;
using Cardamom.Ui;
using Expeditionary.Hexagons;
using Expeditionary.Model.Knowledge;
using Expeditionary.View.Textures;
using OpenTK.Mathematics;

namespace Expeditionary.View.Scenes.Matches.Layers
{
    public class FogOfWarLayer : GraphicsResource, IRenderable
    {
        private static readonly Color4 s_Undiscovered = new(0, 0, 0, 1f);
        private static readonly Color4 s_Hidden = new(0, 0, 0, 0.75f);
        private static readonly Color4 s_Visible = new(0, 0, 0, 0);

        private readonly Vector2i _size;
        private VertexBuffer<Vertex3>? _buffer;
        private readonly RenderShader _shader;
        private readonly PartitionLibrary _partitions;

        internal FogOfWarLayer(
            Vector2i size, VertexBuffer<Vertex3> buffer, RenderShader shader, PartitionLibrary partitions)
        {
            _size = size;
            _buffer = buffer;
            _shader = shader;
            _partitions = partitions;
        }

        public void Set(IPlayerKnowledge knowledge, IEnumerable<Vector3i> delta)
        {
            var options = _partitions.Query().ToArray();
            var vertices = new Vertex3[3];
            foreach (var hex in delta)
            {
                var color = GetColor(knowledge.GetTile(hex));
                foreach ((var corner, var backIndex) in Geometry.GetCorners(hex))
                {
                    var index = GetIndex(corner, _size.Y);
                    var selected = options[index % options.Length];

                    var centerHex = Geometry.GetCornerHex(corner, 0);
                    var leftHex = Geometry.GetCornerHex(corner, 1);
                    var rightHex = Geometry.GetCornerHex(corner, 2);

                    var centerPos = ToVector3(Axial.Cartesian.Instance.Project(centerHex.Xy));
                    var leftPos = ToVector3(Axial.Cartesian.Instance.Project(leftHex.Xy));
                    var rightPos = ToVector3(Axial.Cartesian.Instance.Project(rightHex.Xy));

                    vertices[0] = new(centerPos, color, selected.TexCoords[backIndex][0]);
                    vertices[1] = new(leftPos, color, selected.TexCoords[backIndex][1]);
                    vertices[2] = new(rightPos, color, selected.TexCoords[backIndex][2]);

                    var i = 9 * index + 3 * backIndex;
                    _buffer!.Sub(vertices, i, 3);
                }
            }
        }

        public void SetAll(IPlayerKnowledge knowledge)
        {
            var options = _partitions.Query().ToArray();
            int triangles = 3 * ((_size.X + 2) * (2 * _size.Y + 1) + 1);
            var vertices = new Vertex3[3 * triangles];
            foreach (var corner in Geometry.GetAllCorners(_size))
            {
                var centerHex = Geometry.GetCornerHex(corner, 0);
                var leftHex = Geometry.GetCornerHex(corner, 1);
                var rightHex = Geometry.GetCornerHex(corner, 2);

                var centerPos = ToVector3(Axial.Cartesian.Instance.Project(centerHex.Xy));
                var leftPos = ToVector3(Axial.Cartesian.Instance.Project(leftHex.Xy));
                var rightPos = ToVector3(Axial.Cartesian.Instance.Project(rightHex.Xy));

                var index = GetIndex(corner, _size.Y);
                var selected = options[index % options.Length];
                for (int hex = 0; hex < 3; ++hex)
                {
                    var color = GetColor(knowledge.GetTile(Geometry.GetCornerHex(corner, hex)));
                    var i = 9 * index + 3 * hex;
                    vertices[i] = new(centerPos, color, selected.TexCoords[hex][0]);
                    vertices[i + 1] = new(leftPos, color, selected.TexCoords[hex][1]);
                    vertices[i + 2] = new(rightPos, color, selected.TexCoords[hex][2]);
                }
            }
            _buffer!.Buffer(vertices);
        }

        public void Draw(IRenderTarget target, IUiContext context)
        {
            target.Draw(_buffer!, 0, _buffer!.Length, new(BlendMode.Alpha, _shader, _partitions.GetTexture()));
        }

        public void Initialize() { }

        public void ResizeContext(Vector3 bounds) { }

        public void Update(long delta) { }

        protected override void DisposeImpl()
        {
            _buffer?.Dispose();
            _buffer = null;
        }

        private static Color4 GetColor(SingleTileKnowledge knowledge)
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

        private static Vector3 ToVector3(Vector2 x)
        {
            return new(x.X, 0, x.Y);
        }
    }
}
