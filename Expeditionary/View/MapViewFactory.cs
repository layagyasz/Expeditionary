using Cardamom.Collections;
using Cardamom.Graphics;
using Cardamom.Mathematics.Geometry;
using Expeditionary.Hexagons;
using Expeditionary.Model.Mapping;
using Expeditionary.View.Textures;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Expeditionary.View
{
    public class MapViewFactory
    {
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
        private static readonly Color4 s_GridColor = new(0, 0, 0, 0.25f);
        private static readonly float s_GridWidth = 0.04f;

        private readonly MapViewParameters _parameters;
        private readonly TextureLibrary _textureLibrary;
        private readonly RenderShader _maskShader;
        private readonly RenderShader _texShader;

        public MapViewFactory(
            MapViewParameters parameters, 
            TextureLibrary textureLibrary,
            RenderShader maskShader,
            RenderShader texShader)
        {
            _parameters = parameters;
            _textureLibrary = textureLibrary;
            _maskShader = maskShader;
            _texShader = texShader;
        }

        public MapView Create(Map map, TerrainViewParameters parameters, int seed)
        {
            TerrainLibrary.Option[] options = _textureLibrary.Terrain.Query().ToArray();
            var random = new Random(seed);
            Vertex3[] terrain = new Vertex3[18 * (map.Width - 1) * (map.Height - 1)];
            ArrayList<Vertex3> edges = new();
            int v = 0;
            foreach (var corner in map.GetCorners())
            {
                var centerHex = Geometry.GetCornerHex(corner, 0);
                var center = map.GetTile(centerHex);
                var leftHex = Geometry.GetCornerHex(corner, 1);
                var left = map.GetTile(leftHex);
                var rightHex = Geometry.GetCornerHex(corner, 2);
                var right = map.GetTile(rightHex);
                if (center == null || left == null || right == null)
                {
                    continue;
                }

                var centerPos = ToVector3(Geometry.MapAxial(centerHex.Xy));
                var leftPos = ToVector3(Geometry.MapAxial(leftHex.Xy));
                var rightPos = ToVector3(Geometry.MapAxial(rightHex.Xy));
                var selected = options[random.Next(options.Length)];
                for (int j = 0; j < 3; ++j)
                {
                    var tile = map.GetTile(Geometry.GetCornerHex(corner, j))!;
                    var color = GetTileColor(tile, parameters);
                    terrain[v++] = new(centerPos, color, selected.TexCoords[j][0]);
                    terrain[v++] = new(leftPos, color, selected.TexCoords[j][1]);
                    terrain[v++] = new(rightPos, color, selected.TexCoords[j][2]);
                }
                var edgeA = map.GetEdge(Geometry.GetEdge(centerHex, leftHex))!;
                var edgeB = map.GetEdge(Geometry.GetEdge(leftHex, rightHex))!;
                var edgeC = map.GetEdge(Geometry.GetEdge(centerHex, rightHex))!;
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
                    edges.Add(new(centerPos, parameters.Liquid, selectedEdge.TexCoords[0]));
                    edges.Add(new(leftPos, parameters.Liquid, selectedEdge.TexCoords[1]));
                    edges.Add(new(rightPos, parameters.Liquid, selectedEdge.TexCoords[2]));
                }
            }
            ArrayList<Vertex3> grid = new();
            for (int i=0;i<map.Width;++i)
            {
                for (int j=0;j<map.Height;++j)
                {
                    var centerPos = ToVector3(Geometry.MapAxial(Cubic.HexagonalOffset.Instance.Wrap(new(i, j)).Xy));
                    Shapes.AddVertices(
                        grid,
                        s_GridColor,
                        new Line3(
                            s_Corners.Select(x => x + centerPos).ToArray(), new Vector3(0, 1, 0), isLoop: true),
                        s_GridWidth,
                        center: false);
                }
            }
            return new MapView(
                new VertexBuffer<Vertex3>(grid.GetData(), PrimitiveType.Triangles),
                new VertexBuffer<Vertex3>(terrain, PrimitiveType.Triangles),
                _textureLibrary.Terrain.GetTexture(),
                new VertexBuffer<Vertex3>(edges.GetData(), PrimitiveType.Triangles),
                _textureLibrary.Edges.GetTexture(),
                _maskShader,
                _texShader);
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
