using Cardamom.Collections;
using Cardamom.Graphics;
using Cardamom.Mathematics;
using Expeditionary.Hexagons;
using Expeditionary.Model.Mapping;
using Expeditionary.View.Textures;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Expeditionary.View
{
    public class MapViewFactory
    {
        private static readonly Vector3i[] s_Neighbors =
        {
            new(-1, 1, 0),
            new(-1, 0, 1),
            new(0, -1, 1)
        };

        private readonly MapViewParameters _parameters;
        private readonly TextureLibrary _textureLibrary;
        private readonly RenderShader _shader;

        public MapViewFactory(MapViewParameters parameters, TextureLibrary textureLibrary, RenderShader shader)
        {
            _parameters = parameters;
            _textureLibrary = textureLibrary;
            _shader = shader;
        }

        public MapView Create(Map map, TerrainViewParameters parameters, int seed)
        {
            TerrainLibrary.Option[] options = _textureLibrary.Terrain.Query().ToArray();
            var random = new Random(seed);
            Vertex3[] vertices = new Vertex3[18 * (map.Width - 1) * (map.Height - 1)];
            ArrayList<Vertex3> edgeVertices = new();
            var xRange = new IntInterval(0, map.Width - 1);
            var yRange = new IntInterval(0, map.Height - 1);
            int v = 0;
            for (int x = 1; x < map.Width; ++x)
            {
                for (int y = 0; y < map.Height; ++y)
                {
                    var center = map.GetTile(new(x, y));
                    var centerHex = Cubic.HexagonalOffset.Instance.Wrap(new(x, y));
                    var centerPos = Cartesian.FromAxial(centerHex.Xy);
                    for (int i = 0; i < 2; ++i)
                    {
                        var leftHex = centerHex + s_Neighbors[i];
                        var leftOffset = Cubic.HexagonalOffset.Instance.Project(leftHex);
                        if (!xRange.Contains(leftOffset.X) || !yRange.Contains(leftOffset.Y))
                        {
                            continue;
                        }
                        var rightHex = centerHex + s_Neighbors[i + 1];
                        var rightOffset = Cubic.HexagonalOffset.Instance.Project(rightHex);
                        if (!xRange.Contains(rightOffset.X) || !yRange.Contains(rightOffset.Y))
                        {
                            continue;
                        }

                        var left = map.GetTile(leftOffset);
                        var leftPos = Cartesian.FromAxial(leftHex.Xy);
                        var right = map.GetTile(rightOffset);
                        var rightPos = Cartesian.FromAxial(rightHex.Xy);
                        var selected = options[random.Next(options.Length)];
                        for (int j = 0; j < 3; ++j)
                        {
                            Tile tile;
                            if (j == 0)
                            {
                                tile = center;
                            }
                            else if (j == 1)
                            {
                                tile = left;
                            }
                            else
                            {
                                tile = right;
                            }
                            var color = GetTileColor(tile, parameters);
                            vertices[v++] = new(ToVector3(centerPos), color, selected.TexCoords[j][0]);
                            vertices[v++] = new(ToVector3(leftPos), color, selected.TexCoords[j][1]);
                            vertices[v++] = new(ToVector3(rightPos), color, selected.TexCoords[j][2]);
                        }
                        var edgeA = 
                            map.GetEdge(Cubic.HexagonalOffset.Instance.Project(Geometry.GetEdge(centerHex, leftHex)));
                        var edgeB =
                            map.GetEdge(Cubic.HexagonalOffset.Instance.Project(Geometry.GetEdge(leftHex, rightHex)));
                        var edgeC = 
                            map.GetEdge(Cubic.HexagonalOffset.Instance.Project(Geometry.GetEdge(centerHex, rightHex)));
                        bool[] query = 
                            new bool[] 
                            { 
                                edgeA.Type == Edge.EdgeType.River,
                                edgeB.Type == Edge.EdgeType.River,
                                edgeC.Type == Edge.EdgeType.River 
                            };
                        if (query.Any(x => x))
                        {
                            var edgeOptions = _textureLibrary.Edges.Query(query).ToArray();
                            var selectedEdge = edgeOptions[random.Next(edgeOptions.Length)];
                            edgeVertices.Add(new(ToVector3(centerPos), parameters.Liquid, selectedEdge.TexCoords[0]));
                            edgeVertices.Add(new(ToVector3(leftPos), parameters.Liquid, selectedEdge.TexCoords[1]));
                            edgeVertices.Add(new(ToVector3(rightPos), parameters.Liquid, selectedEdge.TexCoords[2]));
                        }
                    }
                }
            }
            return new MapView(
                new VertexBuffer<Vertex3>(vertices, PrimitiveType.Triangles),
                _textureLibrary.Terrain.GetTexture(),
                new VertexBuffer<Vertex3>(edgeVertices.GetData(), PrimitiveType.Triangles),
                _textureLibrary.Edges.GetTexture(),
                _shader); ;
        }

        private Color4 GetTileColor(Tile tile, TerrainViewParameters parameters)
        {
            var e = (int)(tile.Elevation * _parameters.ElevationLevel) / (_parameters.ElevationLevel - 1f);
            var color = (Color4)Color4.ToHsl(GetBaseTileColor(tile, parameters));
            if (!tile.Terrain.IsLiquid)
            {
                var adj = _parameters.ElevationGradient.Minimum + e *
                    (_parameters.ElevationGradient.Maximum -
                        _parameters.ElevationGradient.Minimum);
                color.B = MathHelper.Clamp(color.B * adj, 0, 1);
            }
            return Color4.FromHsl((Vector4)color);
        }

        private static Color4 GetBaseTileColor(Tile tile, TerrainViewParameters parameters)
        {
            if (tile.Terrain.IsLiquid)
            {
                return parameters.Liquid;
            }
            if (tile.Terrain.Brush.HasValue)
            {
                var brush = tile.Terrain.Brush.Value;
                return Lerp(
                    Lerp(parameters.ColdDry, parameters.HotDry, brush.X),
                    Lerp(parameters.ColdWet, parameters.HotWet, brush.X),
                    brush.Y);
            }
            if (tile.Terrain.Soil.HasValue)
            {
                var soil = tile.Terrain.Soil.Value;
                return (Color4)(soil.X * (Vector4)parameters.Silt
                    + soil.Y * (Vector4)parameters.Clay
                    + soil.Z * (Vector4)parameters.Silt);
            }
            var stone = tile.Terrain.Stone;
            return (Color4)(stone.X * (Vector4)parameters.Stone0
                    + stone.Y * (Vector4)parameters.Stone1
                    + stone.Z * (Vector4)parameters.Stone2);
        }

        private static Color4 Lerp(Color4 a, Color4 b, float v)
        {
            return (Color4)((1 - v) * (Vector4)a + v * (Vector4)b);
        }

        private static Vector3 ToVector3(Vector2 x)
        {
            return new(x.X, 0, x.Y);
        }
    }
}
