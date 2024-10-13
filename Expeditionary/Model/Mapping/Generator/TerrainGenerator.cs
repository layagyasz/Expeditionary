using Cardamom.Collections;
using Cardamom.ImageProcessing;
using Cardamom.ImageProcessing.Pipelines;
using Cardamom.ImageProcessing.Pipelines.Nodes;
using Cardamom.Utils.Suppliers;
using Cardamom.Utils.Suppliers.Matrix;
using Cardamom.Utils.Suppliers.Vector;
using Expeditionary.Hexagons;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Mapping.Generator
{
    public class TerrainGenerator
    {
        public class Parameters
        {
            public int ElevationLevels { get; set; } = 5;
            public float LiquidLevel { get; set; } = 0.25f;
            public Vector3 Stone { get; set; } = new(1, 1, 1);
            public float SoilCover { get; set; } = 0.9f;
            public Vector3 Soil { get; set; } = new(1, 1, 1);
            public float BrushCover { get; set; } = 0.9f;
            public float FoliageCover { get; set; } = 0.6f;
            public float LiquidBonus = 0.2f;
            public int Rivers { get; set; }
        }

        private static readonly int s_Resolution = 1024;
        private static readonly Vector3[] s_Barycenters =
        {
            new(1, 0, 0),
            new(0, 1, 0),
            new(0, 0, 1),
            new(0.5f, 0.5f, 0),
            new(0.5f, 0, 0.5f),
            new(0, 0.5f, 0.5f),
            new(1f / 3f, 1f /3f, 1f / 3f)
        };
        private static readonly Vector2[] s_Centers =
        {
            new(0, 0),
            new(0, 0.5f),
            new(0, 1),
            new(0.5f, 0),
            new(0.5f, 0.5f),
            new(0.5f, 1),
            new(1, 0),
            new(1, 0.5f),
            new(1, 1)
        };

        private readonly ICanvasProvider _canvasProvider =
            new CachingCanvasProvider(new(s_Resolution, s_Resolution), Color4.Black);

        private readonly Pipeline _pipeline;
        private readonly ConstantSupplier<int> _elevationSeed = new();
        private readonly ConstantSupplier<int> _stoneASeed = new();
        private readonly ConstantSupplier<int> _stoneBSeed = new();
        private readonly ConstantSupplier<int> _soilASeed = new();
        private readonly ConstantSupplier<int> _soilBSeed = new();
        private readonly ConstantSupplier<int> _soilCoverSeed = new();
        private readonly ConstantSupplier<int> _temperatureSeed = new();
        private readonly ConstantSupplier<int> _moistureSeed = new();
        private readonly ConstantSupplier<int> _brushCoverSeed = new();
        private readonly ConstantSupplier<int> _foliageCoverSeed = new();

        public TerrainGenerator()
        {
            _pipeline =
                new Pipeline.Builder()
                    .AddNode(new InputNode.Builder().SetKey("position").SetIndex(0))
                    .AddNode(
                        new LatticeNoiseNode.Builder()
                            .SetKey("elevation")
                            .SetInput("input", "position")
                            .SetChannel(Channel.Red)
                            .SetParameters(
                                new()
                                {
                                    Seed = _elevationSeed,
                                    Frequency = new ConstantSupplier<Vector3>(new(.005f, .005f, .005f))
                                }))
                    .AddNode(
                        new LatticeNoiseNode.Builder()
                            .SetKey("stone-a")
                            .SetInput("input", "position")
                            .SetChannel(Channel.Red)
                            .SetParameters(
                                new()
                                {
                                    Seed = _stoneASeed,
                                    Frequency = new ConstantSupplier<Vector3>(new(.02f, .02f, .02f))
                                }))
                    .AddNode(
                        new LatticeNoiseNode.Builder()
                            .SetKey("stone-b")
                            .SetInput("input", "position")
                            .SetOutput("stone-a")
                            .SetChannel(Channel.Green)
                            .SetParameters(
                                new()
                                {
                                    Seed = _stoneBSeed,
                                    Frequency = new ConstantSupplier<Vector3>(new(.02f, .02f, .02f))
                                }))
                    .AddNode(
                        new LatticeNoiseNode.Builder()
                            .SetKey("soil-a")
                            .SetInput("input", "position")
                            .SetChannel(Channel.Red)
                            .SetParameters(
                                new()
                                {
                                    Seed = _soilASeed,
                                    Frequency = new ConstantSupplier<Vector3>(new(.005f, .005f, .005f))
                                }))
                    .AddNode(
                        new LatticeNoiseNode.Builder()
                            .SetKey("soil-b")
                            .SetInput("input", "position")
                            .SetOutput("soil-a")
                            .SetChannel(Channel.Green)
                            .SetParameters(
                                new()
                                {
                                    Seed = _soilBSeed,
                                    Frequency = new ConstantSupplier<Vector3>(new(.005f, .005f, .005f))
                                }))
                    .AddNode(
                        new LatticeNoiseNode.Builder()
                            .SetKey("soil-cover")
                            .SetInput("input", "position")
                            .SetOutput("soil-b")
                            .SetChannel(Channel.Blue)
                            .SetParameters(
                                new()
                                {
                                    Seed = _soilCoverSeed,
                                    Frequency = new ConstantSupplier<Vector3>(new(.05f, .05f, .05f))
                                }))
                                        .AddNode(
                        new LatticeNoiseNode.Builder()
                            .SetKey("temperature")
                            .SetInput("input", "position")
                            .SetChannel(Channel.Red)
                            .SetParameters(
                                new()
                                {
                                    Seed = _temperatureSeed,
                                    Frequency = new ConstantSupplier<Vector3>(new(.005f, .005f, .005f))
                                }))
                    .AddNode(
                        new LatticeNoiseNode.Builder()
                            .SetKey("moisture")
                            .SetInput("input", "position")
                            .SetOutput("temperature")
                            .SetChannel(Channel.Green)
                            .SetParameters(
                                new()
                                {
                                    Seed = _moistureSeed,
                                    Frequency = new ConstantSupplier<Vector3>(new(.005f, .005f, .005f))
                                }))
                    .AddNode(
                        new LatticeNoiseNode.Builder()
                            .SetKey("brush-cover")
                            .SetInput("input", "position")
                            .SetOutput("moisture")
                            .SetChannel(Channel.Blue)
                            .SetParameters(
                                new()
                                {
                                    Seed = _brushCoverSeed,
                                    Frequency = new ConstantSupplier<Vector3>(new(.05f, .05f, .05f))
                                }))
                    .AddNode(
                        new LatticeNoiseNode.Builder()
                            .SetKey("foliage-cover")
                            .SetInput("input", "position")
                            .SetOutput("brush-cover")
                            .SetChannel(Channel.Alpha)
                            .SetParameters(
                                new()
                                {
                                    Seed = _foliageCoverSeed,
                                    Frequency = new ConstantSupplier<Vector3>(new(.05f, .05f, .05f))
                                }))
                    .AddNode(
                        new DenormalizeNode.Builder().SetKey("elevation-denormalize").SetInput("input", "elevation"))
                    .AddNode(new DenormalizeNode.Builder().SetKey("stone-denormalize").SetInput("input", "stone-b"))
                    .AddNode(new DenormalizeNode.Builder().SetKey("soil-denormalize").SetInput("input", "soil-cover"))
                    .AddNode(
                        new DenormalizeNode.Builder().SetKey("plant-denormalize").SetInput("input", "foliage-cover"))
                    .AddNode(
                        new AdjustNode.Builder()
                            .SetKey("elevation-adjust")
                            .SetInput("input", "elevation-denormalize")
                            .SetChannel(Channel.Red)
                            .SetParameters(
                                new()
                                {
                                    Bias =
                                        new Vector4UniformSupplier()
                                        {
                                            ComponentValue = new ConstantSupplier<float>(0.5f)
                                        },
                                    Gradient =
                                        new Matrix4DiagonalUniformSupplier()
                                        {
                                            Diagonal = new ConstantSupplier<float>(0.5f)
                                        }
                                }))
                    .AddNode(
                        new AdjustNode.Builder()
                            .SetKey("stone-adjust")
                            .SetInput("input", "stone-denormalize")
                            .SetChannel(Channel.Color)
                            .SetParameters(
                                new()
                                {
                                    Bias =
                                        new Vector4UniformSupplier()
                                        {
                                            ComponentValue = new ConstantSupplier<float>(0.5f)
                                        },
                                    Gradient =
                                        new Matrix4DiagonalUniformSupplier()
                                        {
                                            Diagonal = new ConstantSupplier<float>(0.5f)
                                        }
                                }))
                    .AddNode(
                        new AdjustNode.Builder()
                            .SetKey("soil-adjust")
                            .SetInput("input", "soil-denormalize")
                            .SetChannel(Channel.Color)
                            .SetParameters(
                                new()
                                {
                                    Bias =
                                        new Vector4UniformSupplier()
                                        {
                                            ComponentValue = new ConstantSupplier<float>(0.5f)
                                        },
                                    Gradient =
                                        new Matrix4DiagonalUniformSupplier()
                                        {
                                            Diagonal = new ConstantSupplier<float>(0.5f)
                                        }
                                }))
                    .AddNode(
                        new AdjustNode.Builder()
                            .SetKey("plant-adjust")
                            .SetInput("input", "plant-denormalize")
                            .SetChannel(Channel.All)
                            .SetParameters(
                                new()
                                {
                                    Bias =
                                        new Vector4UniformSupplier()
                                        {
                                            ComponentValue = new ConstantSupplier<float>(0.5f)
                                        },
                                    Gradient =
                                        new Matrix4DiagonalUniformSupplier()
                                        {
                                            Diagonal = new ConstantSupplier<float>(0.5f)
                                        }
                                }))
                    .AddOutput("elevation-adjust")
                    .AddOutput("stone-adjust")
                    .AddOutput("soil-adjust")
                    .AddOutput("plant-adjust")
                    .Build();
        }

        public void Generate(Parameters parameters, Map map, Random random)
        {
            _elevationSeed.Value = random.Next();
            _stoneASeed.Value = random.Next();
            _stoneBSeed.Value = random.Next();
            _soilASeed.Value = random.Next();
            _soilBSeed.Value = random.Next();
            _soilCoverSeed.Value = random.Next();
            _temperatureSeed.Value = random.Next();
            _moistureSeed.Value = random.Next();
            _brushCoverSeed.Value = random.Next();
            _foliageCoverSeed.Value = random.Next();

            var centers = new Color4[s_Resolution, s_Resolution];
            for (int i = 0; i < map.Width; ++i)
            {
                for (int j = 0; j < map.Height; ++j)
                {
                    var coord = Axial.Cartesian.Instance.Project(Axial.Offset.Instance.Wrap(new(i, j)));
                    centers[i, j] = new(coord.X, coord.Y, 0, 1);
                }
            }
            var input = _canvasProvider.Get();
            input.GetTexture().Update(new(), centers);
            var output = _pipeline.Run(_canvasProvider, input);

            var corners = new float[map.Width + 2, 2 * map.Height + 2];
            var elevation = output[0].GetTexture().GetData();
            var stone = output[1].GetTexture().GetData();
            var soil = output[2].GetTexture().GetData();
            var plants = output[3].GetTexture().GetData();
            _canvasProvider.Return(input);
            foreach (var canvas in output)
            {
                _canvasProvider.Return(canvas);
            }

            Elevation(map, corners, parameters, elevation);
            RiverGenerator.Generate(parameters.Rivers, map, corners, random);
            AdjustMoisture(map, parameters, plants);
            Stone(map, parameters, stone);
            Soil(map, parameters, soil);
            Brush(map, parameters, plants);
            Foliage(map, parameters, plants);
        }

        private static void Elevation(Map map, float[,] corners, Parameters parameters, Color4[,] elevationData)
        {
            for (int i = 0; i < map.Width; ++i)
            {
                for (int j = 0; j < map.Height; ++j)
                {
                    Color4 tileData = elevationData[i, j];
                    var tile = map.GetTile(i, j)!;
                    tile.Elevation = tileData.R;
                    if (tile.Elevation < parameters.LiquidLevel)
                    {
                        tile.Terrain.IsLiquid = true;
                    }
                }
            }
            for (int i = 0; i < map.Width; ++i)
            {
                for (int j = 0; j < map.Height; ++j)
                {
                    var tile = map.GetTile(i, j)!;
                    var coord = Cubic.HexagonalOffset.Instance.Wrap(new(i, j));
                    if (tile.Terrain.IsLiquid)
                    {
                        tile.Slope = 0;
                    }
                    else
                    {
                        tile.Slope =
                            Enumerable.Range(0, 3)
                                .Select(
                                    x =>
                                        Math.Abs(
                                            (map.GetTile(Geometry.GetNeighbor(coord, x)) ?? tile)!.Elevation -
                                            (map.GetTile(Geometry.GetNeighbor(coord, x + 3)) ?? tile)!.Elevation))
                                .Max();
                    }
                }
            }
            for (int i = 0; i < corners.GetLength(0); ++i)
            {
                for (int j = 0; j < corners.GetLength(1); ++j)
                {
                    var corner = Cubic.TriangularOffset.Instance.Wrap(new(i, j));
                    int n = 0;
                    float total = 0;
                    foreach (var option in Geometry.GetCornerHexes(corner)
                            .Select(map.GetTile)
                            .Where(x => x != null)
                            .Select(x => x!.Elevation))
                    {
                        n++;
                        total += option;
                    }
                    corners[i, j] = total / n;
                }
            }
            for (int i = 0; i < map.Width; ++i)
            {
                for (int j = 0; j < map.Height; ++j)
                {
                    var tile = map.GetTile(i, j)!;
                    if (tile.Terrain.IsLiquid)
                    {
                        tile.Elevation = (int)(parameters.ElevationLevels * parameters.LiquidLevel)
                                / (parameters.ElevationLevels - 1f);
                    }
                    else
                    {
                        tile.Elevation = (int)(parameters.ElevationLevels * tile.Elevation)
                                / (parameters.ElevationLevels - 1f);
                    }
                }
            }
        }

        private static void AdjustMoisture(Map map, Parameters parameters, Color4[,] plantData)
        {
            for (int i = 0; i < map.Width; ++i)
            {
                for (int j = 0; j < map.Height; ++j)
                {
                    Color4 tileData = plantData[i, j];
                    var hex = Cubic.HexagonalOffset.Instance.Wrap(new(i, j));
                    if (Geometry.GetEdges(hex)
                            .Select(map.GetEdge)
                            .Where(x => x != null)
                            .Any(x => x!.Levels.ContainsKey(Edge.EdgeType.River))
                        || Geometry.GetNeighbors(hex)
                            .Select(map.GetTile)
                            .Where(x => x != null)
                            .Any(x => x!.Terrain.IsLiquid))
                    {
                        plantData[i, j].G += parameters.LiquidBonus;
                    }
                    var tile = map.GetTile(i, j)!;
                    tile.Heat = plantData[i, j].R;
                    tile.Moisture = plantData[i, j].G;
                }
            }
        }

        private static void Stone(Map map, Parameters parameters, Color4[,] stoneData)
        {
            for (int i = 0; i < map.Width; ++i)
            {
                for (int j = 0; j < map.Height; ++j)
                {
                    Color4 tileData = stoneData[i, j];
                    var stone = GetBarycentric(tileData.R, tileData.G, parameters.Stone);
                    map.GetTile(i, j)!.Terrain.Stone = GetMaxComponent(stone);
                }
            }
        }

        private static void Soil(Map map, Parameters parameters, Color4[,] soilData)
        {
            for (int i = 0; i < map.Width; ++i)
            {
                for (int j = 0; j < map.Height; ++j)
                {
                    Color4 tileData = soilData[i, j];
                    var tile = map.GetTile(i, j)!;
                    if (tileData.B + tile.Slope < parameters.SoilCover)
                    {
                        var soilModifier = new Vector3(1 - tile.Elevation, 1, 2 - tile.Slope - tile.Elevation);
                        var soil =
                            GetCenter(
                                GetBarycentric(tileData.R, tileData.G, parameters.Soil * soilModifier), s_Barycenters);
                        tile.Terrain.Soil = soil;
                    }
                }
            }
        }

        private static void Brush(Map map, Parameters parameters, Color4[,] plantData)
        {
            for (int i = 0; i < map.Width; ++i)
            {
                for (int j = 0; j < map.Height; ++j)
                {
                    Color4 tileData = plantData[i, j];
                    var tile = map.GetTile(i, j);
                    if (tile!.Terrain.Soil.HasValue && tileData.B < parameters.BrushCover)
                    {
                        tile.Terrain.Brush = GetCenter(new(tile.Heat, tile.Moisture), s_Centers);
                    }
                }
            }
        }

        private static void Foliage(Map map, Parameters parameters, Color4[,] plantData)
        {
            for (int i = 0; i < map.Width; ++i)
            {
                for (int j = 0; j < map.Height; ++j)
                {
                    Color4 tileData = plantData[i, j];
                    var tile = map.GetTile(i, j);
                    if (!tile!.Terrain.IsLiquid && tile!.Terrain.Soil.HasValue && tileData.A < parameters.FoliageCover)
                    {
                        tile.Terrain.Foliage = GetCenter(new(tile.Heat, tile.Moisture), s_Centers);
                    }
                }
            }
        }

        private static float Distance(Vector3 left, Vector3 right)
        {
            var diff = left - right;
            return Math.Max(diff.X, Math.Max(diff.Y, diff.Z));
        }

        private static float Distance(Vector2 left, Vector2 right)
        {
            return Math.Abs(left.X - right.X) + Math.Abs(left.Y - right.Y);
        }

        private static Vector3 GetCenter(Vector3 x, Vector3[] options)
        {
            return options.ArgMin(y => Distance(x, y));
        }

        private static Vector2 GetCenter(Vector2 x, Vector2[] options)
        {
            return options.ArgMin(y => Distance(x, y));
        }

        private static Vector3 GetBarycentric(float a, float b, Vector3 weight)
        {
            if (a + b > 1)
            {
                a = 1 - a;
                b = 1 - b;
            }
            var weighted = weight * new Vector3(a, b, 1 - a - b);
            return weighted / (weighted.X + weighted.Y + weighted.Z);
        }

        private static Vector3 GetMaxComponent(Vector3 x)
        {
            if (x.X > x.Y && x.X > x.Z)
            {
                return new(1, 0, 0);
            }
            else if (x.Y > x.Z)
            {
                return new(0, 1, 0);
            }
            return new(0, 0, 1);
        }
    }
}
