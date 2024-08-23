using Cardamom.Collections;
using Cardamom.ImageProcessing;
using Cardamom.ImageProcessing.Pipelines;
using Cardamom.ImageProcessing.Pipelines.Nodes;
using Cardamom.Mathematics.Coordinates.Projections;
using Cardamom.Utils.Suppliers;
using Cardamom.Utils.Suppliers.Matrix;
using Cardamom.Utils.Suppliers.Vector;
using Expeditionary.Hexagons;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Mapping
{
    public class MapGenerator
    {
        private static readonly int s_Resolution = 1024;
        private static readonly Vector2i[] s_Neighbors =
        {
            new(0, -1),
            new(1, -1),
            new(1, 0),
            new(0, 1),
            new(-1, 1),
            new(-1, 0)
        };
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

        private static readonly IProjection<Vector2i, Vector2i> _axialOffset = new Axial.Offset();

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

        public MapGenerator()
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
                                    Frequency = new ConstantSupplier<Vector3>(new(.01f, .01f, .01f))
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
                                    Frequency = new ConstantSupplier<Vector3>(new(.005f, .005f, .005f))
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
                        new DenormalizeNode.Builder().SetKey("elevation-denormalize").SetInput("input", "elevation"))
                    .AddNode(new DenormalizeNode.Builder().SetKey("stone-denormalize").SetInput("input", "stone-b"))
                    .AddNode(new DenormalizeNode.Builder().SetKey("soil-denormalize").SetInput("input", "soil-cover"))
                    .AddNode(
                        new DenormalizeNode.Builder().SetKey("plant-denormalize").SetInput("input", "brush-cover"))
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
                    .AddOutput("elevation-adjust")
                    .AddOutput("stone-adjust")
                    .AddOutput("soil-adjust")
                    .AddOutput("plant-adjust")
                    .Build();
        }

        public Map Generate(TerrainParameters parameters, Vector2i size, int seed)
        {
            var random = new Random(seed);
            _elevationSeed.Value = random.Next();
            _stoneASeed.Value = random.Next();
            _stoneBSeed.Value = random.Next();
            _soilASeed.Value = random.Next();
            _soilBSeed.Value = random.Next();
            _soilCoverSeed.Value = random.Next();
            _temperatureSeed.Value = random.Next();
            _moistureSeed.Value = random.Next();
            _brushCoverSeed.Value = random.Next();
            
            var centers = new Color4[s_Resolution, s_Resolution];
            for (int i=0; i<size.X; ++i)
            {
                for (int j=0; j<size.Y; ++j)
                {
                    var coord = Cartesian.FromOffset(new(i, j));
                    centers[i, j] = new(coord.X, coord.Y, 0, 1);
                }
            }
            var input = _canvasProvider.Get();
            input.GetTexture().Update(new(), centers);
            var output = _pipeline.Run(_canvasProvider, input);

            var tiles = new Tile[size.X, size.Y];
            Initialize(tiles);
            Elevation(tiles, parameters, output[0].GetTexture().GetData());
            Stone(tiles, parameters, output[1].GetTexture().GetData());
            Soil(tiles, parameters, output[2].GetTexture().GetData());
            Brush(tiles, parameters, output[3].GetTexture().GetData());

            _canvasProvider.Return(input);
            foreach (var canvas in output)
            {
                _canvasProvider.Return(canvas);
            }

            var builder = new Map.Builder(size);
            for (int i=0; i<size.X; ++i)
            {
                for (int j=0; j<size.Y; ++j)
                {
                    builder.Set(new(i, j), tiles[i, j]);
                }
            }
            return builder.Build();
        }

        private static void Initialize(Tile[,] tiles)
        {
            for (int i = 0; i < tiles.GetLength(0); ++i)
            {
                for (int j = 0; j < tiles.GetLength(1); ++j)
                {
                    tiles[i, j] = new();
                }
            }
        }

        private static void Elevation(Tile[,] tiles, TerrainParameters parameters, Color4[,] elevationData)
        {
            for (int i = 0; i < tiles.GetLength(0); ++i)
            {
                for (int j = 0; j < tiles.GetLength(1); ++j)
                {
                    Color4 tileData = elevationData[i, j];
                    tiles[i, j].Elevation = tileData.R;
                    if (tileData.R < parameters.LiquidLevel)
                    {
                        tiles[i, j].Terrain.IsLiquid = true;
                    }
                }
            }
            float min = float.PositiveInfinity;
            float max = float.NegativeInfinity;
            float sum = 0;
            for (int i = 0; i < tiles.GetLength(0); ++i)
            {
                for (int j = 0; j < tiles.GetLength(1); ++j)
                {
                    var tile = tiles[i, j];
                    var coord = _axialOffset.Wrap(new(i, j));
                    tile.Slope =  
                        Enumerable.Range(0, 3)
                            .Select(
                                x => 
                                    Math.Abs(
                                        GetNeighborOrTile(tile, coord + s_Neighbors[x], tiles, t => t.Elevation) - 
                                        GetNeighborOrTile(tile, coord + s_Neighbors[x + 3], tiles, t => t.Elevation)))
                            .Max();
                    sum += tile.Slope;
                    min = Math.Min(min, tile.Slope);
                    max = Math.Max(max, tile.Slope);
                }
            }
        }

        private static void Stone(Tile[,] tiles, TerrainParameters parameters, Color4[,] stoneData)
        {
            for (int i = 0; i < tiles.GetLength(0); ++i)
            {
                for (int j = 0; j < tiles.GetLength(1); ++j)
                {
                    Color4 tileData = stoneData[i, j];
                    var stone = GetBarycentric(tileData.R, tileData.G, parameters.Stone);
                    tiles[i, j].Terrain.Stone = GetMaxComponent(stone);
                }
            }
        }

        private static void Soil(Tile[,] tiles, TerrainParameters parameters, Color4[,] soilData)
        {
            for (int i = 0; i < tiles.GetLength(0); ++i)
            {
                for (int j = 0; j < tiles.GetLength(1); ++j)
                {
                    Color4 tileData = soilData[i, j];
                    var tile = tiles[i, j];
                    if (Math.Max(tileData.B, Math.Max(tile.Elevation, tile.Slope)) < parameters.SoilCover)
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

        private static void Brush(Tile[,] tiles, TerrainParameters parameters, Color4[,] plantData)
        {
            for (int i = 0; i < tiles.GetLength(0); ++i)
            {
                for (int j = 0; j < tiles.GetLength(1); ++j)
                {
                    Color4 tileData = plantData[i, j];
                    if (tiles[i,j].Terrain.Soil.HasValue && tileData.B < parameters.BrushCover)
                    {
                        var brush = GetCenter(new(tileData.R, tileData.G), s_Centers);
                        tiles[i, j].Terrain.Brush = brush;
                    }
                }
            }
        }

        private static T GetNeighborOrTile<T>(Tile tile, Vector2i neighbor, Tile[,] tiles, Func<Tile, T> readFn)
        {
            var coord = _axialOffset.Project(neighbor);
            if (coord.X < 0 || coord.Y < 0 || coord.X >= tiles.GetLength(0) || coord.Y >= tiles.GetLength(1))
            {
                return readFn(tile);
            }
            return readFn(tiles[coord.X, coord.Y]);
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
