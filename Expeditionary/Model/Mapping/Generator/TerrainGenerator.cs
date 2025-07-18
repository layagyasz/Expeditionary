using Cardamom.Collections;
using Cardamom.ImageProcessing;
using Cardamom.ImageProcessing.Filters;
using Cardamom.ImageProcessing.Pipelines;
using Cardamom.ImageProcessing.Pipelines.Nodes;
using Cardamom.Mathematics;
using Cardamom.Utils.Suppliers;
using Cardamom.Utils.Suppliers.Matrix;
using Cardamom.Utils.Suppliers.Vector;
using Expeditionary.Hexagons;
using MathNet.Numerics.Statistics;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Mapping.Generator
{
    public class TerrainGenerator
    {
        public record class Parameters
        {
            public LayerParameters ElevationLayer { get; set; } = new();
            public LayerParameters StoneLayer { get; set; } = new();
            public LayerParameters SoilLayer { get; set; } = new();
            public LayerParameters GroundCoverLayer { get; set; } = new();
            public LayerParameters SoilCoverLayer { get; set; } = new();
            public LayerParameters TemperatureLayer { get; set; } = new();
            public LayerParameters MoistureLayer { get; set; } = new();
            public LayerParameters BrushLayer { get; set; } = new();
            public LayerParameters FoliageLayer { get; set; } = new();
            public int ElevationLevels { get; set; }
            public float LiquidLevel { get; set; }
            public Vector3 Stone { get; set; }
            public float SoilCover { get; set; }
            public SoilParameters SoilA { get; set; } = new();
            public SoilParameters SoilB { get; set; } = new();
            public SoilParameters SoilC { get; set; } = new();
            public float GroundCoverCover { get; set; }
            public SoilParameters GroundCover { get; set; } = new();
            public float BrushCover { get; set; }
            public float FoliageCover { get; set; }
            public bool BrushRequireSoil { get; set; }
            public bool FoliageRequireSoil { get; set; }
            public float LiquidMoistureBonus { get; set; }
            public float RiverDensity { get; set; }
            public Interval MoistureRange { get; set; }
            public Interval TemperatureRange { get; set; }
        }

        public record class LayerParameters 
        {
            public LatticeNoise.Settings Noise { get; set; } = new();
            public Quadratic Transform { get; set; } = new();
            public float Mean { get; set; }
            public float StandardDeviation { get; set; }
        }

        public record class SoilParameters
        {
            public float Weight { get; set; }
            public Quadratic ElevationWeight { get; set; }
            public Quadratic SlopeWeight { get; set; }
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

        public static void Generate(Parameters parameters, Map map, Random random)
        {
            using var canvasProvider = new CachingCanvasProvider(new(s_Resolution, s_Resolution), Color4.Black);
            var pipeline =
                new Pipeline.Builder()
                    .AddNode(new InputNode.Builder().SetKey("position").SetIndex(0))
                    .AddNode(
                        new LatticeNoiseNode.Builder()
                            .SetKey("elevation")
                            .SetInput("input", "position")
                            .SetChannel(Channel.Red)
                            .SetParameters(ToNoiseParameters(parameters.ElevationLayer.Noise, random)))
                    .AddNode(
                        new LatticeNoiseNode.Builder()
                            .SetKey("stone-a")
                            .SetInput("input", "position")
                            .SetChannel(Channel.Red)
                            .SetParameters(ToNoiseParameters(parameters.StoneLayer.Noise, random)))
                    .AddNode(
                        new LatticeNoiseNode.Builder()
                            .SetKey("stone-b")
                            .SetInput("input", "position")
                            .SetOutput("stone-a")
                            .SetChannel(Channel.Green)
                            .SetParameters(ToNoiseParameters(parameters.StoneLayer.Noise, random)))
                    .AddNode(
                        new LatticeNoiseNode.Builder()
                            .SetKey("soil-a")
                            .SetInput("input", "position")
                            .SetChannel(Channel.Red)
                            .SetParameters(ToNoiseParameters(parameters.SoilLayer.Noise, random)))
                    .AddNode(
                        new LatticeNoiseNode.Builder()
                            .SetKey("soil-b")
                            .SetInput("input", "position")
                            .SetOutput("soil-a")
                            .SetChannel(Channel.Green)
                            .SetParameters(ToNoiseParameters(parameters.SoilLayer.Noise, random)))
                    .AddNode(
                        new LatticeNoiseNode.Builder()
                            .SetKey("soil-cover")
                            .SetInput("input", "position")
                            .SetOutput("soil-b")
                            .SetChannel(Channel.Blue)
                            .SetParameters(ToNoiseParameters(parameters.SoilCoverLayer.Noise, random)))
                    .AddNode(
                        new LatticeNoiseNode.Builder()
                            .SetKey("ground-cover")
                            .SetInput("input", "position")
                            .SetOutput("soil-cover")
                            .SetChannel(Channel.Alpha)
                            .SetParameters(ToNoiseParameters(parameters.GroundCoverLayer.Noise, random)))
                    .AddNode(
                        new LatticeNoiseNode.Builder()
                            .SetKey("temperature")
                            .SetInput("input", "position")
                            .SetChannel(Channel.Red)
                            .SetParameters(ToNoiseParameters(parameters.TemperatureLayer.Noise, random)))
                    .AddNode(
                        new LatticeNoiseNode.Builder()
                            .SetKey("moisture")
                            .SetInput("input", "position")
                            .SetOutput("temperature")
                            .SetChannel(Channel.Green)
                            .SetParameters(ToNoiseParameters(parameters.MoistureLayer.Noise, random)))
                    .AddNode(
                        new LatticeNoiseNode.Builder()
                            .SetKey("brush-cover")
                            .SetInput("input", "position")
                            .SetOutput("moisture")
                            .SetChannel(Channel.Blue)
                            .SetParameters(ToNoiseParameters(parameters.BrushLayer.Noise, random)))
                    .AddNode(
                        new LatticeNoiseNode.Builder()
                            .SetKey("foliage-cover")
                            .SetInput("input", "position")
                            .SetOutput("brush-cover")
                            .SetChannel(Channel.Alpha)
                            .SetParameters(ToNoiseParameters(parameters.FoliageLayer.Noise, random)))
                    .AddNode(
                        new DenormalizeNode.Builder()
                            .SetKey("elevation-denormalize")
                            .SetInput("input", "elevation")
                            .SetParameters(
                                new DenormalizeNode.Parameters()
                                {
                                    Mean =
                                        new Vector4UniformSupplier()
                                        {
                                            ComponentValue = 
                                                new ConstantSupplier<float>(parameters.ElevationLayer.Mean)
                                        },
                                    StandardDeviation =
                                        new Vector4UniformSupplier()
                                        {
                                            ComponentValue = 
                                                new ConstantSupplier<float>(
                                                    parameters.ElevationLayer.StandardDeviation)
                                        },
                                }))
                    .AddNode(
                        new DenormalizeNode.Builder()
                            .SetKey("stone-denormalize")
                            .SetInput("input", "stone-b")
                            .SetParameters(
                                new DenormalizeNode.Parameters()
                                {
                                    Mean =
                                        new Vector4UniformSupplier()
                                        {
                                            ComponentValue = new ConstantSupplier<float>(parameters.StoneLayer.Mean)
                                        },
                                    StandardDeviation =
                                        new Vector4UniformSupplier()
                                        {
                                            ComponentValue = 
                                                new ConstantSupplier<float>(parameters.StoneLayer.StandardDeviation)
                                        },
                                }))
                    .AddNode(
                        new DenormalizeNode.Builder()
                            .SetKey("soil-denormalize")
                            .SetInput("input", "ground-cover")
                            .SetParameters(
                                new DenormalizeNode.Parameters()
                                {
                                    Mean =
                                        new ConstantSupplier<Vector4>(
                                            new(
                                                parameters.SoilLayer.Mean,
                                                parameters.SoilLayer.Mean,
                                                parameters.SoilCoverLayer.Mean,
                                                parameters.GroundCoverLayer.Mean)),
                                    StandardDeviation =
                                        new ConstantSupplier<Vector4>(
                                            new(
                                                parameters.SoilLayer.StandardDeviation,
                                                parameters.SoilLayer.StandardDeviation,
                                                parameters.SoilCoverLayer.StandardDeviation, 
                                                parameters.GroundCoverLayer.StandardDeviation)),
                                }))
                    .AddNode(
                        new DenormalizeNode.Builder()
                            .SetKey("plant-denormalize")
                            .SetInput("input", "foliage-cover")
                            .SetParameters(
                                new DenormalizeNode.Parameters()
                                {
                                    Mean =
                                        new ConstantSupplier<Vector4>(
                                            new(
                                                parameters.TemperatureLayer.Mean,
                                                parameters.MoistureLayer.Mean,
                                                parameters.BrushLayer.Mean,
                                                parameters.FoliageLayer.Mean)),
                                    StandardDeviation =
                                        new ConstantSupplier<Vector4>(
                                            new(
                                                parameters.TemperatureLayer.StandardDeviation,
                                                parameters.MoistureLayer.StandardDeviation,
                                                parameters.BrushLayer.StandardDeviation,
                                                parameters.FoliageLayer.StandardDeviation)),
                                }))
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
                                            ComponentValue = 
                                                new ConstantSupplier<float>(parameters.ElevationLayer.Transform.C)
                                        },
                                    Gradient =
                                        new Matrix4DiagonalUniformSupplier()
                                        {
                                            Diagonal = 
                                                new ConstantSupplier<float>(parameters.ElevationLayer.Transform.B)
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
                                            ComponentValue = 
                                                new ConstantSupplier<float>(parameters.StoneLayer.Transform.C)
                                        },
                                    Gradient =
                                        new Matrix4DiagonalUniformSupplier()
                                        {
                                            Diagonal = new ConstantSupplier<float>(parameters.StoneLayer.Transform.B)
                                        }
                                }))
                    .AddNode(
                        new AdjustNode.Builder()
                            .SetKey("soil-adjust")
                            .SetInput("input", "soil-denormalize")
                            .SetChannel(Channel.All)
                            .SetParameters(
                                new()
                                {
                                    Bias =
                                        new ConstantSupplier<Vector4>(
                                            new(
                                                parameters.SoilLayer.Transform.C,
                                                parameters.SoilLayer.Transform.C,
                                                parameters.SoilCoverLayer.Transform.C,
                                                parameters.GroundCoverLayer.Transform.C)),
                                    Gradient = new Matrix4DiagonalVectorSupplier()
                                    {
                                        Diagonal = 
                                            new ConstantSupplier<Vector4>(
                                                new(
                                                    parameters.SoilLayer.Transform.B,
                                                    parameters.SoilLayer.Transform.B,
                                                    parameters.SoilCoverLayer.Transform.B,
                                                    parameters.GroundCoverLayer.Transform.B))
                                    },
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
                                        new ConstantSupplier<Vector4>(
                                            new(
                                                parameters.TemperatureLayer.Transform.C 
                                                    * (parameters.TemperatureRange.Maximum 
                                                        + parameters.TemperatureRange.Minimum),
                                                parameters.MoistureLayer.Transform.C 
                                                    * (parameters.MoistureRange.Maximum
                                                        + parameters.MoistureRange.Minimum),
                                                parameters.BrushLayer.Transform.C,
                                                parameters.FoliageLayer.Transform.C)),
                                    Gradient = new Matrix4DiagonalVectorSupplier()
                                    {
                                        Diagonal =
                                            new ConstantSupplier<Vector4>(
                                                new(
                                                    parameters.TemperatureLayer.Transform.B 
                                                        * (parameters.TemperatureRange.Maximum
                                                            - parameters.TemperatureRange.Minimum),
                                                    parameters.MoistureLayer.Transform.B 
                                                        * (parameters.MoistureRange.Maximum
                                                            - parameters.MoistureRange.Minimum),
                                                    parameters.BrushLayer.Transform.B,
                                                    parameters.FoliageLayer.Transform.B))
                                    }
                                }))
                    .AddOutput("elevation-adjust")
                    .AddOutput("stone-adjust")
                    .AddOutput("soil-adjust")
                    .AddOutput("plant-adjust")
                    .Build();

            var centers = new Color4[s_Resolution, s_Resolution];
            for (int i = 0; i < map.Width; ++i)
            {
                for (int j = 0; j < map.Height; ++j)
                {
                    var coord = Axial.Cartesian.Instance.Project(Axial.Offset.Instance.Wrap(new(i, j)));
                    centers[i, j] = new(coord.X, coord.Y, 0, 1);
                }
            }
            var input = canvasProvider.Get();
            input.GetTexture().Update(new(), centers);
            var output = pipeline.Run(canvasProvider, input);

            var elevationData = output[0].GetTexture().GetData();
            var stoneData = output[1].GetTexture().GetData();
            var soilData = output[2].GetTexture().GetData();
            var plantData = output[3].GetTexture().GetData();
            canvasProvider.Return(input);
            foreach (var canvas in output)
            {
                canvasProvider.Return(canvas);
            }

            var elevation = new float[map.Width, map.Height];
            var slope = new float[map.Width, map.Height];
            var corners = new float[map.Width + 2, 2 * map.Height + 2];

            Elevation(map, elevation, slope, corners, parameters, elevationData);
            var numRivers = (int)(parameters.RiverDensity * map.Height * map.Width * (1 - parameters.LiquidLevel));
            RiverGenerator.Generate(numRivers, map, corners, random);
            AdjustMoisture(map, parameters, plantData);
            Stone(map, parameters, stoneData);
            Soil(map, parameters, soilData, elevation, slope);
            GroundCover(map, parameters, soilData, elevation, slope);
            Brush(map, parameters, plantData);
            Foliage(map, parameters, plantData);
        }

        private static LatticeNoiseNode.Parameters ToNoiseParameters(LatticeNoise.Settings settings, Random random)
        {
            return new()
            {
                Seed = new ConstantSupplier<int>(random.Next()),
                Frequency = new ConstantSupplier<Vector3>(settings.Frequency),
                Lacunarity = new ConstantSupplier<Vector3>(settings.Lacunarity),
                Octaves = new ConstantSupplier<int>(settings.Octaves),
                Persistence = new ConstantSupplier<float>(settings.Persistence),
                Amplitude = new ConstantSupplier<float>(settings.Amplitude),
                Evaluator = new ConstantSupplier<LatticeNoise.Evaluator>(settings.Evaluator),
                Interpolator = new ConstantSupplier<LatticeNoise.Interpolator>(settings.Interpolator),
                PreTreatment = new ConstantSupplier<LatticeNoise.Treatment>(settings.PreTreatment),
                PostTreatment = new ConstantSupplier<LatticeNoise.Treatment>(settings.PostTreatment)
            };
        }

        private static void Elevation(
            Map map,
            float[,] elevation,
            float[,] slope, 
            float[,] corners,
            Parameters parameters,
            Color4[,] elevationData)
        {
            float[] elevations = new float[map.Width * map.Height];
            for (int i = 0; i < map.Width; ++i)
            {
                for (int j = 0; j < map.Height; ++j)
                {
                    Color4 tileData = elevationData[i, j];
                    var tile = map.Get(i, j)!;
                    elevation[i, j] = tileData.R;
                    elevations[i * map.Height + j] = tileData.R;
                }
            }
            Array.Sort(elevations);
            // DebugElevations(elevations);

            var liquidLevel = elevations[(int)(parameters.LiquidLevel * (elevations.Length - 1))];
            for (int i = 0; i < map.Width; ++i)
            {
                for (int j = 0; j < map.Height; ++j)
                {
                    elevation[i, j] = (elevation[i, j] - liquidLevel) / (1f - liquidLevel);
                }
            }
            for (int i = 0; i < map.Width; ++i)
            {
                for (int j = 0; j < map.Height; ++j)
                {
                    var tile = map.Get(i, j)!;
                    var coord = Cubic.HexagonalOffset.Instance.Wrap(new(i, j));
                    if (elevation[i, j] < 0)
                    {
                        elevation[i, j] = 0;
                        tile.Terrain.IsLiquid = true;
                        slope[i, j] = 0;
                    }
                    else
                    {
                        for (int k = 0; k < 3; ++k)
                        {
                            var left = Cubic.HexagonalOffset.Instance.Project(Geometry.GetNeighbor(coord, k));
                            var right = Cubic.HexagonalOffset.Instance.Project(Geometry.GetNeighbor(coord, k + 3));
                            left = map.Contains(left) ? left : new(i, j);
                            right = map.Contains(right) ? right : new(i, j);
                            slope[i, j] = Math.Abs(elevation[left.X, left.Y] - elevation[right.X, right.Y]);
                        }
                    }
                }
            }
            for (int i = 0; i < map.Width; ++i)
            {
                for (int j = 0; j < map.Height; ++j)
                {
                    var tile = map.Get(i, j)!;
                    tile.Elevation = (int)(parameters.ElevationLevels * elevation[i, j]);
                }
            }
            for (int i = 0; i < corners.GetLength(0); ++i)
            {
                for (int j = 0; j < corners.GetLength(1); ++j)
                {
                    var corner = Cubic.TriangularOffset.Instance.Wrap(new(i, j));
                    corners[i, j] =
                        Geometry.GetCornerHexes(corner)
                            .Select(Cubic.HexagonalOffset.Instance.Project)
                            .Where(map.Contains)
                            .Sum(x => elevation[x.X, x.Y]);
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
                            .Any(x => x!.Levels.ContainsKey(EdgeType.River))
                        || Geometry.GetNeighbors(hex)
                            .Select(map.Get)
                            .Where(x => x != null)
                            .Any(x => x!.Terrain.IsLiquid))
                    {
                        plantData[i, j].G += parameters.LiquidMoistureBonus;
                    }
                    var tile = map.Get(i, j)!;
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
                    map.Get(i, j)!.Terrain.Stone = GetMaxComponent(stone);
                }
            }
        }

        private static void Soil(
            Map map, Parameters parameters, Color4[,] soilData, float[,] elevation, float[,] slope)
        {
            for (int i = 0; i < map.Width; ++i)
            {
                for (int j = 0; j < map.Height; ++j)
                {
                    Color4 tileData = soilData[i, j];
                    var tile = map.Get(i, j)!;
                    var e = elevation[i, j];
                    var s = slope[i, j];
                    if (tileData.B - 0.25f * (e - 0.5f) < parameters.SoilCover)
                    {
                        var modifiers = 
                            new Vector3(
                                EvaluateSoil(parameters.SoilA, e, s),
                                EvaluateSoil(parameters.SoilB, e, s), 
                                EvaluateSoil(parameters.SoilC, e, s));
                        var weights = 
                            new Vector3(parameters.SoilA.Weight, parameters.SoilB.Weight, parameters.SoilC.Weight);
                        var soil =
                            GetCenter(
                                GetBarycentric(tileData.R, tileData.G, weights * modifiers), s_Barycenters);
                        tile.Terrain.Soil = soil;
                    }
                }
            }
        }

        private static void GroundCover(
            Map map, Parameters parameters, Color4[,] soilData, float[,] elevation, float[,] slope)
        {
            if (parameters.GroundCoverCover < float.Epsilon)
            {
                return;
            }
            for (int i = 0; i < map.Width; ++i)
            {
                for (int j = 0; j < map.Height; ++j)
                {
                    var tile = map.Get(i, j)!;
                    if (tile.Terrain.IsLiquid)
                    {
                        continue;
                    }
                    Color4 tileData = soilData[i, j];
                    var e = elevation[i, j];
                    var s = slope[i, j];
                    tile.Terrain.HasGroundCover = 
                        tileData.A - EvaluateSoil(parameters.GroundCover, e, s) < parameters.GroundCoverCover;
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
                    var tile = map.Get(i, j);
                    if (!tile!.Terrain.IsLiquid 
                        && (!parameters.BrushRequireSoil || tile!.Terrain.Soil.HasValue)
                        && tileData.B < parameters.BrushCover)
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
                    var tile = map.Get(i, j);
                    if (!tile!.Terrain.IsLiquid 
                        && (!parameters.FoliageRequireSoil || tile!.Terrain.Soil.HasValue) 
                        && tileData.A < parameters.FoliageCover)
                    {
                        tile.Terrain.Foliage = GetCenter(new(tile.Heat, tile.Moisture), s_Centers);
                    }
                }
            }
        }

        private static float EvaluateSoil(SoilParameters parameters, float elevation, float slope)
        {
            return parameters.ElevationWeight.Evaluate(elevation) + parameters.SlopeWeight.Evaluate(slope);
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

        private static void DebugElevations(float[] elevations)
        {
            int[] buckets = new int[10];
            foreach (var e in elevations)
            {
                int i = 
                    MathHelper.Clamp((int)(e * 0.5f * buckets.Length + 0.5f * buckets.Length), 0, buckets.Length - 1);
                buckets[i]++;
            }
            Console.WriteLine(string.Join(",", buckets));
            Console.WriteLine(Statistics.MeanStandardDeviation(elevations));
        }
    }
}
