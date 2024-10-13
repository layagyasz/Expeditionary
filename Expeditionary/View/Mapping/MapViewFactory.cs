using Cardamom.Collections;
using Cardamom.Graphics;
using Cardamom.Mathematics.Geometry;
using Expeditionary.Hexagons;
using Expeditionary.Model.Mapping;
using Expeditionary.View.Common.Buffers;
using Expeditionary.View.Textures;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Expeditionary.View.Mapping
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
            PartitionLibrary.Option[] options = _textureLibrary.Partitions.Query().ToArray();
            var random = new Random(seed);
            int triangles = 6 * (map.Width - 1) * (map.Height - 1);
            var bufferBuilder =
                new LayeredVertexBuffer.Builder()
                    .SetRenderResources(GetRenderResources)
                    .AddLayer(3 * triangles)
                    .AddLayer(3 * triangles)
                    .AddLayer(3 * triangles)
                    .AddLayer(triangles)
                    .AddLayer(18 * map.Width * map.Height);

            int triangle = 0;
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

                var centerPos = ToVector3(Axial.Cartesian.Instance.Project(centerHex.Xy));
                var leftPos = ToVector3(Axial.Cartesian.Instance.Project(leftHex.Xy));
                var rightPos = ToVector3(Axial.Cartesian.Instance.Project(rightHex.Xy));
                for (int layer = 0; layer < 3; ++layer)
                {
                    var selected = options[random.Next(options.Length)];
                    for (int hex = 0; hex < 3; ++hex)
                    {
                        var tile = map.GetTile(Geometry.GetCornerHex(corner, hex))!;
                        var color = GetTileColor(tile, layer, parameters);
                        var index = 9 * triangle + 3 * hex;
                        bufferBuilder.SetVertex(layer, index, new(centerPos, color, selected.TexCoords[hex][0]));
                        bufferBuilder.SetVertex(layer, index + 1, new(leftPos, color, selected.TexCoords[hex][1]));
                        bufferBuilder.SetVertex(layer, index + 2, new(rightPos, color, selected.TexCoords[hex][2]));
                    }
                }

                var edgeA = map.GetEdge(Geometry.GetEdge(centerHex, leftHex))!;
                var edgeB = map.GetEdge(Geometry.GetEdge(leftHex, rightHex))!;
                var edgeC = map.GetEdge(Geometry.GetEdge(centerHex, rightHex))!;
                bool[] query =
                    new bool[]
                    {
                            edgeA.Levels.ContainsKey(Edge.EdgeType.River),
                            edgeB.Levels.ContainsKey(Edge.EdgeType.River),
                            edgeC.Levels.ContainsKey(Edge.EdgeType.River)
                    };
                if (query.Any(x => x))
                {
                    var edgeOptions = _textureLibrary.Edges.Query(query).ToArray();
                    var selectedEdge = edgeOptions[random.Next(edgeOptions.Length)];
                    bufferBuilder.SetVertex(
                        layer: 3, 3 * triangle, new(centerPos, parameters.Liquid, selectedEdge.TexCoords[0]));
                    bufferBuilder.SetVertex(
                        layer: 3, 3 * triangle + 1, new(leftPos, parameters.Liquid, selectedEdge.TexCoords[1]));
                    bufferBuilder.SetVertex(
                        layer: 3, 3 * triangle + 2, new(rightPos, parameters.Liquid, selectedEdge.TexCoords[2]));
                }

                ++triangle;
            }

            StructureLayerCreator.Create(bufferBuilder, map, _textureLibrary.Structures, random);

            ArrayList<Vertex3> grid = new();
            for (int i = 0; i < map.Width; ++i)
            {
                for (int j = 0; j < map.Height; ++j)
                {
                    var centerPos = ToVector3(Axial.Cartesian.Instance.Project(Axial.Offset.Instance.Wrap(new(i, j))));
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
                bufferBuilder.Build(),
                _maskShader);

        }

        private Color4 GetTileColor(Tile tile, int layer, TerrainViewParameters parameters)
        {
            var color = (Color4)Color4.ToHsv(GetBaseTileColor(tile, layer, parameters));
            if (!tile.Terrain.IsLiquid)
            {
                var adj = _parameters.ElevationGradient.Minimum + tile.Elevation *
                    (_parameters.ElevationGradient.Maximum -
                        _parameters.ElevationGradient.Minimum);
                color.B = MathHelper.Clamp(color.B * adj, 0, 1);
            }
            return Color4.FromHsv((Vector4)color);
        }

        private RenderResources GetRenderResources(int layer)
        {
            // Structures
            if (layer == 4)
            {
                return new(BlendMode.Alpha, _texShader, _textureLibrary.Structures.GetTexture());
            }
            // Rivers
            if (layer == 3)
            {
                return new(BlendMode.Alpha, _texShader, _textureLibrary.Edges.GetTexture());
            }
            return new(BlendMode.Alpha, _texShader, _textureLibrary.Partitions.GetTexture());
        }

        private static Color4 GetBaseTileColor(Tile tile, int layer, TerrainViewParameters parameters)
        {
            // City
            // TODO: this should be rendered with special tiles
            if (layer == 2)
            {
                switch (tile.Structure.Type)
                {
                    case StructureType.Agricultural:
                        return new(0f, 1f, 0f, 1f);
                    case StructureType.Mining:
                        return new(1f, 1f, 0f, 1f);
                    case StructureType.Residential:
                        return new(0f, 1f, 1f, 1f);
                    case StructureType.Commercial:
                        return new(0f, 0f, 1f, 1f);
                    case StructureType.Industrial:
                        return new(1f, 0f, 0f, 1f);
                    default:
                        return new();
                }
            }

            // Foliage
            if (layer == 1)
            {
                return tile.Terrain.Foliage.HasValue
                    ? parameters.Foliage.Interpolate(tile.Terrain.Foliage.Value) : new();
            }

            // Base
            // Ocean
            if (tile.Terrain.IsLiquid)
            {
                return parameters.Liquid;
            }
            // Brush
            if (tile.Terrain.Brush.HasValue)
            {
                return parameters.Brush.Interpolate(tile.Terrain.Brush.Value);
            }
            // Soil
            if (tile.Terrain.Soil.HasValue)
            {
                return parameters.Soil.Interpolate(tile.Terrain.Soil.Value);
            }
            return parameters.Stone.Interpolate(tile.Terrain.Stone);
        }

        private static Vector3 ToVector3(Vector2 x)
        {
            return new(x.X, 0, x.Y);
        }
    }
}
