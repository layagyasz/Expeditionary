using Cardamom.Graphics;
using Expeditionary.Hexagons;
using Expeditionary.Model.Mapping;
using Expeditionary.View.Textures;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Expeditionary.View
{
    public class FogOfWarLayerFactory
    {
        private readonly RenderShader _shader;
        private readonly PartitionLibrary _partitions;

        public FogOfWarLayerFactory(RenderShader shader, PartitionLibrary partitions)
        {
            _shader = shader;
            _partitions = partitions;
        }

        public FogOfWarLayer Create(Map map, int seed)
        {
            var random = new Random(seed);
            PartitionLibrary.Option[] options = _partitions.Query().ToArray();
            int triangles = 6 * (map.Width + 2) * (map.Height + 1);
            var vertices = new Vertex3[3 * triangles];
            foreach (var corner in map.GetCorners())
            {
                var centerHex = Geometry.GetCornerHex(corner, 0);
                var leftHex = Geometry.GetCornerHex(corner, 1);
                var rightHex = Geometry.GetCornerHex(corner, 2);

                var centerPos = ToVector3(Axial.Cartesian.Instance.Project(centerHex.Xy));
                var leftPos = ToVector3(Axial.Cartesian.Instance.Project(leftHex.Xy));
                var rightPos = ToVector3(Axial.Cartesian.Instance.Project(rightHex.Xy));

                var selected = options[random.Next(options.Length)];
                var index = 9 * GetIndex(corner, map.Height);
                for (int hex = 0; hex < 3; ++hex)
                {
                    var color = Color4.Black;
                    var i = index + 3 * hex;
                    vertices[i] = new(centerPos, color, selected.TexCoords[hex][0]);
                    vertices[i + 1] = new(leftPos, color, selected.TexCoords[hex][1]);
                    vertices[i + 2] = new(rightPos, color, selected.TexCoords[hex][2]);
                }
            }
            return new(
                new VertexBuffer<Vertex3>(PrimitiveType.Triangles), vertices, _shader, _partitions.GetTexture());
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
