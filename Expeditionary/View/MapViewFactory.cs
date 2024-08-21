using Cardamom.Graphics;
using Cardamom.Mathematics;
using Expeditionary.Coordinates;
using Expeditionary.Model.Mapping;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Expeditionary.View
{
    public class MapViewFactory
    {
        private static readonly Axial2i[] s_Neighbors =
        {
            new(-1, 1),
            new(-1, 0),
            new(0, -1)
        };

        private readonly RenderShader _tileBaseShader;

        public MapViewFactory(RenderShader tileBaseShader)
        {
            _tileBaseShader = tileBaseShader;
        }

        public MapView Create(Map map)
        {
            Vertex3[] vertices = new Vertex3[6 * (map.Width - 1) * (map.Height - 1)];
            var xRange = new IntInterval(0, map.Width - 1);
            var yRange = new IntInterval(0, map.Height - 1);
            int v = 0;
            for (int x = 1; x < map.Width; ++x)
            {
                for (int y = 0; y < map.Height; ++y)
                {
                    var centerAxial = Offset2i.ToAxial(new(x, y));
                    var center = Axial2i.ToCartesian(centerAxial);
                    for (int i = 0; i < 2; ++i)
                    {
                        var left = centerAxial + s_Neighbors[i];
                        var leftOffset = Axial2i.ToOffset(left);
                        if (!xRange.Contains(leftOffset.X) || !yRange.Contains(leftOffset.Y))
                        {
                            continue;
                        }
                        var right = centerAxial + s_Neighbors[i + 1];
                        var rightOffset = Axial2i.ToOffset(right);
                        if (!xRange.Contains(rightOffset.X) || !yRange.Contains(rightOffset.Y))
                        {
                            continue;
                        }
                        vertices[v++] = new(ToVector3(center), Color4.Red, new());
                        vertices[v++] = new(ToVector3(Axial2i.ToCartesian(left)), Color4.Green, new());
                        vertices[v++] = new(ToVector3(Axial2i.ToCartesian(right)), Color4.Blue, new());
                    }
                }
            }
            return new MapView(new VertexBuffer<Vertex3>(vertices, PrimitiveType.Triangles), _tileBaseShader);
        }

        private static Vector3 ToVector3(Vector2 x)
        {
            return new(x.X, 0, x.Y);
        }
    }
}
