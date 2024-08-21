using Cardamom.ImageProcessing;
using Cardamom.ImageProcessing.Pipelines;
using Cardamom.ImageProcessing.Pipelines.Nodes;
using Cardamom.Utils.Suppliers;
using Cardamom.Utils.Suppliers.Matrix;
using Cardamom.Utils.Suppliers.Vector;
using Expeditionary.Coordinates;
using MathNet.Numerics.Distributions;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Mapping
{
    public class MapGenerator
    {
        private static readonly int s_Resolution = 1024;
        private static readonly float s_Mean = 0;
        private static readonly float s_StdDev = 0.2f;

        private readonly ICanvasProvider _canvasProvider = 
            new CachingCanvasProvider(new(s_Resolution, s_Resolution), Color4.Black);

        public Map Generate(TerrainParameters parameters, Vector2i size, int seed)
        {
            var random = new Random(seed);
            var pipeline =
                new Pipeline.Builder()
                    .AddNode(new InputNode.Builder().SetKey("position").SetIndex(0))
                    .AddNode(
                        new LatticeNoiseNode.Builder()
                            .SetKey("stone-a")
                            .SetInput("input", "position")
                            .SetChannel(Channel.Red)
                            .SetParameters(
                                new()
                                {
                                    Seed = new FuncSupplier<int>(random.Next)
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
                                    Seed = new FuncSupplier<int>(random.Next)
                                }))
                    .AddNode(new DenormalizeNode.Builder().SetKey("stone-denormalize").SetInput("input", "stone-b"))
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
                    .AddOutput("stone-adjust")
                    .Build();
            
            var centers = new Color4[s_Resolution, s_Resolution];
            var tiles = new Tile[size.X, size.Y];
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
            var output = pipeline.Run(_canvasProvider, input)[0];
            var data = output.GetTexture().GetData();

            for (int i=0; i<size.X; ++i)
            {
                for (int j=0; j<size.Y; ++j)
                {
                    Color4 stoneData = data[i, j];
                    var stone = GetBarycentric(stoneData.R, stoneData.G, parameters.StoneParameters!.Weight);
                    tiles[i, j] = new Tile(new(GetMaxComponent(stone)));
                }
            }
            _canvasProvider.Return(input);

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
