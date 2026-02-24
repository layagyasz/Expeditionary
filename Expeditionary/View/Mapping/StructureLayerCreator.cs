using Expeditionary.Hexagons;
using Expeditionary.Model.Mapping;
using Expeditionary.View.Common.Buffers;
using Expeditionary.View.Textures;
using OpenTK.Mathematics;

namespace Expeditionary.View.Mapping
{
    public class StructureLayerCreator
    {
        private static readonly int s_LayerId = 5;
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

        public static void Create(
            LayeredVertexBuffer.Builder bufferBuilder, Map map, StructureLibrary structures, Random random)
        {
            int h = 0;
            for (int i=0; i<map.Width; ++i)
            {
                for (int j=0; j<map.Height; ++j)
                {
                    var hex = Cubic.HexagonalOffset.Instance.Wrap(new(i, j));

                    var connections = new StructureLibrary.IConnectionQuery[6];
                    int e = 0;
                    foreach (var edge in Geometry.GetEdges(hex).Select(map.GetEdge))
                    {
                        var query = new List<(StructureLibrary.ConnectionType, int)>();
                        if (edge != null && edge.Levels.TryGetValue(EdgeType.Road, out var level))
                        {
                            query.Add((StructureLibrary.ConnectionType.Road, level));
                        }
                        connections[e++] = new StructureLibrary.OpenConnectionQuery(query);
                    }

                    // TODO -- Generate textures for building types
                    var options =
                        structures.Query(
                            new StructureLibrary.StructureQuery(StructureType.None, Level: 0, connections))
                        .ToList();
                    var selected = options[random.Next(options.Count)];

                    AddHex(bufferBuilder, h++, hex, selected);
                }
            }
        }

        private static void AddHex(
            LayeredVertexBuffer.Builder bufferBuilder, int index, Vector3i hex, StructureLibrary.Option option)
        {
            var hexCenter = ToVector3(Cubic.Cartesian.Instance.Project(hex));
            for (int i=0; i<6; ++i)
            {
                AddTriangle(bufferBuilder, 18 * index, hexCenter, i, option);
            }
        }

        private static void AddTriangle(
            LayeredVertexBuffer.Builder bufferBuilder,
            int index,
            Vector3 hexCenter,
            int triangle,
            StructureLibrary.Option option)
        {
            bufferBuilder.SetVertex(
                s_LayerId, index + 3 * triangle, new(hexCenter, Color4.White, option.TexCenter, new()));
            bufferBuilder.SetVertex(
                s_LayerId,
                index + 3 * triangle + 1, 
                new(
                    hexCenter + s_Corners[triangle], 
                    Color4.White, 
                    option.TexCoords[triangle],
                    new()));
            bufferBuilder.SetVertex(
                s_LayerId,
                index + 3 * triangle + 2,
                new(
                    hexCenter + s_Corners[(triangle + 1) % 6],
                    Color4.White,
                    option.TexCoords[(triangle + 1) % 6], 
                    new()));
        }

        private static Vector3 ToVector3(Vector2 x)
        {
            return new(x.X, 0, x.Y);
        }
    }
}
