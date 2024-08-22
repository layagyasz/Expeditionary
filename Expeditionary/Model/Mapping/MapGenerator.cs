using Cardamom.ImageProcessing;
using Cardamom.ImageProcessing.Pipelines;
using Cardamom.ImageProcessing.Pipelines.Nodes;
using Cardamom.Utils.Suppliers;
using Cardamom.Utils.Suppliers.Matrix;
using Cardamom.Utils.Suppliers.Vector;
using Expeditionary.Coordinates;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Mapping
{
    public class MapGenerator
    {
        private static readonly int s_Resolution = 1024;
        private static readonly Axial2i[] s_Neighbors =
        {
            new(0, -1),
            new(1, -1),
            new(1, 0),
            new(0, 1),
            new(-1, 1),
            new(-1, 0)
        };

        private readonly ICanvasProvider _canvasProvider = 
            new CachingCanvasProvider(new(s_Resolution, s_Resolution), Color4.Black);

        private readonly Pipeline _pipeline;
        private readonly ConstantSupplier<int> _elevationSeed = new();
        private readonly ConstantSupplier<int> _stoneASeed = new();
        private readonly ConstantSupplier<int> _stoneBSeed = new();

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
                                    Frequency = new ConstantSupplier<Vector3>(new(.05f, .05f, .05f))
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
                                    Frequency = new ConstantSupplier<Vector3>(new(.05f, .05f, .05f))
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
                                    Frequency = new ConstantSupplier<Vector3>(new(.05f, .05f, .05f))
                                }))
                    .AddNode(
                        new DenormalizeNode.Builder().SetKey("elevation-denormalize").SetInput("input", "elevation"))
                    .AddNode(new DenormalizeNode.Builder().SetKey("stone-denormalize").SetInput("input", "stone-b"))
                    .AddNode(
                        new AdjustNode.Builder()
                            .SetKey("elevation-adjust")
                            .SetInput("input", "elevation-denormalize")
                            .SetChannel(Channel.Red)
                            .SetParameters(new AdjustNode.Parameters()
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
                            .SetParameters(new AdjustNode.Parameters()
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
                    .Build();
        }

        public Map Generate(TerrainParameters parameters, Vector2i size, int seed)
        {
            var random = new Random(seed);
            _elevationSeed.Value = random.Next();
            _stoneASeed.Value = random.Next();
            _stoneBSeed.Value = random.Next();
            
            var centers = new Color4[s_Resolution, s_Resolution];
            for (int i=0; i<size.X; ++i)
            {
                for (int j=0; j<size.Y; ++j)
                {
                    var coord = Offset2i.ToCartesian(new(i, j));
                    centers[i, j] = new(coord.X, coord.Y, 0, 1);
                }
            }
            var input = _canvasProvider.Get();
            input.GetTexture().Update(new(), centers);
            var output = _pipeline.Run(_canvasProvider, input);

            var tiles = new Tile[size.X, size.Y];
            Initialize(tiles);
            Elevation(tiles, output[0].GetTexture().GetData());
            Stone(tiles, parameters, output[1].GetTexture().GetData());


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

        private static void Elevation(Tile[,] tiles, Color4[,] elevationData)
        {
            for (int i = 0; i < tiles.GetLength(0); ++i)
            {
                for (int j = 0; j < tiles.GetLength(1); ++j)
                {
                    Color4 tileData = elevationData[i, j];
                    tiles[i, j].Elevation = tileData.R;
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
                    var coord = Offset2i.ToAxial(new(i, j));
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
            Console.WriteLine("{0} < {1} < {2}", min, sum / (tiles.GetLength(0) * tiles.GetLength(1)), max);
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

        private static T GetNeighborOrTile<T>(Tile tile, Axial2i neighbor, Tile[,] tiles, Func<Tile, T> readFn)
        {
            var coord = Axial2i.ToOffset(neighbor);
            if (coord.X < 0 || coord.Y < 0 || coord.X >= tiles.GetLength(0) || coord.Y >= tiles.GetLength(1))
            {
                return readFn(tile);
            }
            return readFn(tiles[coord.X, coord.Y]);
        }

        private static Barycentric2f GetBarycentric(float a, float b, Barycentric2f weight)
        {
            if (a + b > 1)
            {
                a = 1 - a;
                b = 1 - b;
            }
            return Barycentric2f.Normalize(weight * new Barycentric2f(a, b, 1 - a - b));
        }

        private static int GetMaxComponent(Barycentric2f x)
        {
            if (x.A > x.B && x.A > x.C)
            {
                return 0;
            }
            else if (x.B > x.C)
            {
                return 1;
            }
            return 2;
        }
    }
}
