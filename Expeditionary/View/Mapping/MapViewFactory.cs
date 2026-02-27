using Cardamom.Collections;
using Cardamom.Graphics;
using Cardamom.Mathematics.Geometry;
using Expeditionary.Hexagons;
using Expeditionary.Model.Mapping;
using Expeditionary.View.Common;
using Expeditionary.View.Common.Buffers;
using Expeditionary.View.Textures;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Expeditionary.View.Mapping
{
    public class MapViewFactory
    {
        private static readonly int RidgeLayerId = 2;
        private static readonly int RiverLayerId = 3;

        private static readonly float Sqrt3d2 = 0.5f * MathF.Sqrt(3);
        private static readonly Vector3[] Corners =
        {
            new(-0.5f, 0, -Sqrt3d2),
            new(0.5f, 0, -Sqrt3d2),
            new(1, 0, 0),
            new(0.5f, 0, Sqrt3d2),
            new(-0.5f, 0, Sqrt3d2),
            new(-1, 0, 0)
        };
        private static readonly Color4 GridColor = new(0, 0, 0, 0.25f);
        private static readonly float GridWidth = 0.04f;

        private readonly MapViewParameters _parameters;
        private readonly MapTextureLibrary _textureLibrary;
        private readonly RenderShader _filterShader;
        private readonly RenderShader _maskShader;

        public MapViewFactory(
            MapViewParameters parameters,
            MapTextureLibrary textureLibrary,
            RenderShader filterShader,
            RenderShader maskShader)
        {
            _parameters = parameters;
            _textureLibrary = textureLibrary;
            _filterShader = filterShader;
            _maskShader = maskShader;
        }

        public MapView Create(Map map, TerrainViewParameters parameters, int seed)
        {
            PartitionLibrary.Option[] options = _textureLibrary.Partitions.Query().ToArray();
            var random = new Random(seed);
            int triangles = 6 * (map.Width - 1) * (map.Height - 1);
            var bufferBuilder =
                new LayeredVertexBuffer.Builder()
                    .AddLayer(3 * triangles)
                    .AddLayer(3 * triangles)
                    .AddLayer(triangles)
                    .AddLayer(triangles)
                    .AddLayer(18 * map.Width * map.Height);

            int triangle = 0;
            foreach (var corner in Geometry.GetAllCorners(map.Size))
            {
                var centerHex = Geometry.GetCornerHex(corner, 0);
                var center = map.Get(centerHex);
                var leftHex = Geometry.GetCornerHex(corner, 1);
                var left = map.Get(leftHex);
                var rightHex = Geometry.GetCornerHex(corner, 2);
                var right = map.Get(rightHex);
                if (center == null || left == null || right == null)
                {
                    continue;
                }

                var centerPos = ToVector3(Axial.Cartesian.Instance.Project(centerHex.Xy));
                var leftPos = ToVector3(Axial.Cartesian.Instance.Project(leftHex.Xy));
                var rightPos = ToVector3(Axial.Cartesian.Instance.Project(rightHex.Xy));

                // Base Terrain
                var terrainMask = options[random.Next(options.Length)];
                for (int hex = 0; hex < 3; ++hex)
                {
                    var tile = map.Get(Geometry.GetCornerHex(corner, hex))!;
                    var color = GetTileColor(tile, 0, parameters, map.ElevationLevels);
                    var index = 9 * triangle + 3 * hex;
                    bufferBuilder.SetVertex(0, index, new(centerPos, color, terrainMask.TexCoords[hex][0], new()));
                    bufferBuilder.SetVertex(0, index + 1, new(leftPos, color, terrainMask.TexCoords[hex][1], new()));
                    bufferBuilder.SetVertex(0, index + 2, new(rightPos, color, terrainMask.TexCoords[hex][2], new()));
                }

                // Foliage
                Vector3i foliageQuery =
                    2 * new Vector3i(
                        Convert.ToInt32(center.Terrain.Foliage.HasValue),
                        Convert.ToInt32(left.Terrain.Foliage.HasValue),
                        Convert.ToInt32(right.Terrain.Foliage.HasValue));
                Vector2[] foliage;
                if (foliageQuery.ManhattanLength > 0)
                {
                    var foliageOptions = _textureLibrary.Foliage.Query(foliageQuery).ToArray();
                    foliage = foliageOptions[random.Next(foliageOptions.Length)].TexCoords;
                }
                else
                {
                    foliage = new Vector2[] { Vector2.Zero, Vector2.Zero, Vector2.Zero };
                }
                var foliageMask = options[random.Next(options.Length)];
                for (int hex = 0; hex < 3; ++hex)
                {
                    var tile = map.Get(Geometry.GetCornerHex(corner, hex))!;
                    var color = GetTileColor(tile, 0, parameters, map.ElevationLevels);
                    var index = 9 * triangle + 3 * hex;
                    bufferBuilder.SetVertex(
                        1, index, new(centerPos, color, foliageMask.TexCoords[hex][0], foliage[0]));
                    bufferBuilder.SetVertex(
                        1, index + 1, new(leftPos, color, foliageMask.TexCoords[hex][1], foliage[1]));
                    bufferBuilder.SetVertex(
                        1, index + 2, new(rightPos, color, foliageMask.TexCoords[hex][2], foliage[2]));
                }

                var edgeA = map.GetEdge(Geometry.GetEdge(centerHex, leftHex))!;
                var edgeB = map.GetEdge(Geometry.GetEdge(leftHex, rightHex))!;
                var edgeC = map.GetEdge(Geometry.GetEdge(centerHex, rightHex))!;

                // Rivers
                AddEdge(
                    bufferBuilder, 
                    RiverLayerId, 
                    triangle,
                    parameters.Liquid,
                    centerPos, 
                    leftPos, 
                    rightPos,
                    random,
                    _textureLibrary.Rivers,
                    new bool[]
                    {
                        edgeA.Levels.ContainsKey(EdgeType.River),
                        edgeB.Levels.ContainsKey(EdgeType.River),
                        edgeC.Levels.ContainsKey(EdgeType.River)
                    });

                // Ridges
                AddEdge(
                    bufferBuilder,
                    RidgeLayerId,
                    triangle,
                    GetRidgeColor(center, left, right, parameters, map.ElevationLevels),
                    centerPos,
                    leftPos,
                    rightPos,
                    random,
                    _textureLibrary.Ridges,
                    new bool[]
                    {
                        center.Elevation != left.Elevation,
                        left.Elevation != right.Elevation,
                        center.Elevation != right.Elevation
                    });

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
                        GridColor,
                        new Line3(
                            Corners.Select(x => x + centerPos).ToArray(), new Vector3(0, 1, 0), isLoop: true),
                        GridWidth,
                        center: false);
                }
            }
            return new MapView(
                new VertexBuffer<Vertex3>(grid.GetData(), PrimitiveType.Triangles),
                bufferBuilder.Build(),
                _maskShader,
                _filterShader,
                _textureLibrary);
        }

        private Color4 GetTileColor(Tile tile, int layer, TerrainViewParameters parameters, int elevationLevels)
        {
            if (!tile.Terrain.IsLiquid)
            {
                return Color4.FromHsv(Colors.CombineHsv(
                    Color4.ToHsv(GetBaseTileColor(tile, layer, parameters)),
                    new(
                        0, 
                        1,
                        MathHelper.Lerp(
                            _parameters.ElevationGradient.Minimum,
                            _parameters.ElevationGradient.Maximum,
                            1f * tile.Elevation / elevationLevels), 
                        1)));
            }
            return GetBaseTileColor(tile, layer, parameters);
        }

        private Color4 GetRidgeColor(Tile a, Tile b, Tile c, TerrainViewParameters parameters, int elevationLevels)
        {
            var tile = a;
            if (b.Elevation > tile.Elevation)
            {
                tile = b;
            }
            if (c.Elevation > tile.Elevation)
            {
                tile = c;
            }
            return Color4.FromHsv(Colors.CombineHsv(
                Color4.ToHsv(GetTileColor(tile, layer: 0, parameters, elevationLevels)), _parameters.RidgeShift));
        }

        private static void AddEdge(
            LayeredVertexBuffer.Builder builder,
            int layerId,
            int index,
            Color4 color,
            Vector3 a,
            Vector3 b,
            Vector3 c,
            Random random,
            EdgeLibrary library,
            bool[] query)
        {
            if (query.Any(x => x))
            {
                var edgeOptions = library.Query(query).ToArray();
                var selectedEdge = edgeOptions[random.Next(edgeOptions.Length)];
                builder.SetVertex(layerId, 3 * index, new(a, color, selectedEdge.TexCoords[0], new()));
                builder.SetVertex(layerId, 3 * index + 1, new(b, color, selectedEdge.TexCoords[1], new()));
                builder.SetVertex(layerId, 3 * index + 2, new(c, color, selectedEdge.TexCoords[2], new()));
            }
        }

        private static Color4 GetBaseTileColor(Tile tile, int layer, TerrainViewParameters parameters)
        {
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
            // Ground Cover
            if (tile.Terrain.HasGroundCover)
            {
                return parameters.GroundCover;
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
