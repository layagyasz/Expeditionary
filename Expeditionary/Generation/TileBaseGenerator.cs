using Cardamom.Graphics;
using Cardamom.Graphics.TexturePacking;
using Cardamom.ImageProcessing.Pipelines.Nodes;
using Cardamom.ImageProcessing.Pipelines;
using Cardamom.Utils.Suppliers;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using Cardamom.ImageProcessing;

namespace Expeditionary.Generation
{
    public class TileBaseGenerator
    {
        private static readonly Color4[] s_Masks =
        {
            new(1f, 0f, 0f, 1f),
            new(0f, 1f, 0f, 1f),
            new(0f, 0f, 1f, 1f)
        };

        private readonly RenderShader _partitionShader;

        public TileBaseGenerator(RenderShader partitionShader)
        {
            _partitionShader = partitionShader;
        }

        public ITextureVolume Generate(float attenuation, int seed)
        {
            var random = new Random(seed);
            var noise =
                new Pipeline.Builder()
                    .AddNode(new GeneratorNode.Builder().SetKey("new"))
                    .AddNode(
                        new GradientNode.Builder()
                            .SetKey("gradient")
                            .SetInput("input", "new")
                            .SetChannel(Channel.Color)
                            .SetParameters(
                                new GradientNode.Parameters()
                                {
                                    Scale = ConstantSupplier<Vector2>.Create(
                                        new Vector2(1f / 128, 1f / 128)),
                                    Gradient = ConstantSupplier<Matrix4x2>.Create(
                                        new Matrix4x2(new(1, 0), new(0, 1), new(), new()))
                                }))
                    .AddNode(
                        new LatticeNoiseNode.Builder()
                            .SetKey("red")
                            .SetInput("input", "gradient")
                            .SetChannel(Channel.Red)
                            .SetParameters(
                                new()
                                {
                                    Seed = new FuncSupplier<int>(random.Next)
                                }))
                    .AddNode(
                        new LatticeNoiseNode.Builder()
                            .SetKey("green")
                            .SetInput("input", "gradient")
                            .SetOutput("red")
                            .SetChannel(Channel.Green)
                            .SetParameters(
                                new()
                                {
                                    Seed = new FuncSupplier<int>(random.Next)
                                }))
                    .AddOutput("green")
                    .Build();
            var canvasProvider = new CachingCanvasProvider(new(64, 64), Color4.Black);

            var renderTexture = new RenderTexture(new(64, 64));
            var h = 0.5f * MathF.Sqrt(3);
            var verts = new Vertex3[]
            {
                new(new(0f, h, 0f), new(1f, 0f, 0f, 1f), new(0f, 1f)),
                new(new(0.5f, 0f, 0f), new(0f, 1f, 0f, 1f), new(0f, 0f)),
                new(new(1f, h, 0f), new(0f, 0f, 1f, 1f), new(1f, 1f))
            };
            _partitionShader.SetFloat("attenuation", attenuation);
            renderTexture.PushProjection(new(-10, Matrix4.CreateOrthographicOffCenter(0, 1, 0, 1, -10, 10)));
            renderTexture.PushViewMatrix(Matrix4.Identity);
            renderTexture.PushModelMatrix(Matrix4.Identity);
            var sheet =
                new DynamicTextureVolume(
                    new DynamicStaticSizeTexturePage.Supplier(
                        new(1024, 1024), new(64, 64), Color4.Black, new()), checkAllPages: false);
            for (int i = 0; i < 85; ++i)
            {
                var result = noise.Run(canvasProvider)[0];
                var texture = result.GetTexture();
                canvasProvider.Return(result);
                for (int j = 0; j < s_Masks.Length; ++j)
                {
                    _partitionShader.SetColor("mask", s_Masks[j]);
                    renderTexture.Clear();
                    renderTexture.Draw(
                        verts,
                        PrimitiveType.Triangles,
                        0,
                        verts.Length,
                        new(BlendMode.None, _partitionShader, texture));
                    renderTexture.Display();
                    var key = string.Format("tile-base-{0}-{1}", i, j);
                    sheet.Add(key, renderTexture.CopyTexture());
                }
            }
            foreach (var texture in sheet.GetTextures())
            {
                texture.CopyToImage().SaveToFile(string.Format("{0}.png", texture.GetHashCode()));
            }
            canvasProvider.Dispose();
            return sheet;
        }
    }
}
