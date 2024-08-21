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
        private static readonly Color4[] s_Colors =
        {
            Color4.Red,
            Color4.Green,
            Color4.Blue
        };

        private readonly TerrainTextureLibrary _tileBaseLibrary;
        private readonly RenderShader _tileBaseShader;

        public MapViewFactory(TerrainTextureLibrary tileBaseLibrary, RenderShader tileBaseShader)
        {
            _tileBaseLibrary = tileBaseLibrary;
            _tileBaseShader = tileBaseShader;
        }

        public MapView Create(Map map, int seed)
        {
            TerrainTextureLibrary.Option[] options = _tileBaseLibrary.Query().ToArray();
            var random = new Random(seed);
            Vertex3[] vertices = new Vertex3[18 * (map.Width - 1) * (map.Height - 1)];
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
                        var selected = options[random.Next(options.Length)];
                        for (int j = 0; j < 3; ++j)
                        {
                            Color4 color;
                            if (j==0)
                            {
                                color = ChooseColor(centerAxial);
                            } 
                            else if (j==1)
                            {
                                color = ChooseColor(left);
                            }
                            else
                            {
                                color = ChooseColor(right);
                            }
                            vertices[v++] = new(ToVector3(center), color, selected.TexCoords[j][0]);
                            vertices[v++] = new(ToVector3(Axial2i.ToCartesian(left)), color, selected.TexCoords[j][1]);
                            vertices[v++] =
                                new(ToVector3(Axial2i.ToCartesian(right)), color, selected.TexCoords[j][2]);
                        }
                    }
                }
            }
            return new MapView(
                new VertexBuffer<Vertex3>(vertices, PrimitiveType.Triangles),
                _tileBaseLibrary.GetTexture(), 
                _tileBaseShader);
        }

        private Color4 ChooseColor(Axial2i position)
        {
            return s_Colors[Math.Abs(HashCode.Combine(position.Q, position.R)) % 3];
        }

        private static Vector3 ToVector3(Vector2 x)
        {
            return new(x.X, 0, x.Y);
        }
    }
}
