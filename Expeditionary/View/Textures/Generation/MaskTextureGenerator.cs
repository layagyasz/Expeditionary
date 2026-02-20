using Cardamom.Graphics;
using Cardamom.Graphics.TexturePacking;
using Cardamom.ImageProcessing.Pipelines.Nodes;
using Cardamom.ImageProcessing.Pipelines;
using Cardamom.Utils.Suppliers;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using Cardamom.ImageProcessing;
using Cardamom.Mathematics;

namespace Expeditionary.View.Textures.Generation
{
    public class MaskTextureGenerator
    {
        private record class Distribution(Vector4 Mask, Vector3i Levels);

        private static readonly float s_Sqrt3 = MathF.Sqrt(3);
        private static readonly Distribution[] s_Distributions =
        {
            new(new(1f, 1f, 1f, 1f), new(2, 2, 2)),
            new(new(1f, 1f, 0f, 1f), new(2, 2, 0)),
            new(new(1f, 0f, 0f, 1f), new(2, 0, 0)),

            new(new(1f, 0.5f, 0.5f, 1f), new(2, 1, 1)),
            new(new(1f, 0.5f, 0f, 1f), new(2, 1, 0)),

            new(new(0.5f, 0.5f, 0.5f, 1f), new(1, 1, 1)),
            new(new(0.5f, 0.5f, 0f, 1f), new(1, 1, 0)),
            new(new(0.5f, 0f, 0f, 1f), new(1, 0, 0)),
        };
        private static readonly Vector4 s_Offset = new(-0.2f, -0.2f, -0.2f, 0f);

        private readonly RenderShader _maskShader;
        private readonly Pipeline _pipeline;

        private readonly ConstantSupplier<int> _aSeed = new();
        private readonly ConstantSupplier<int> _wSeed = new();
        private readonly ConstantSupplier<Vector3> _frequency = new();

        public MaskTextureGenerator(RenderShader maskShader)
        {
            _maskShader = maskShader;
            _pipeline =
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
                                    Seed = _aSeed,
                                    Frequency = _frequency
                                }))
                    .AddNode(new DenormalizeNode.Builder().SetKey("denormalize").SetInput("input", "red"))
                    .AddNode(
                        new WhiteNoiseNode.Builder()
                            .SetKey("blue")
                            .SetInput("input", "gradient")
                            .SetOutput("denormalize")
                            .SetChannel(Channel.Blue)
                            .SetParameters(
                                new()
                                {
                                    Seed = _wSeed
                                }))
                    .AddOutput("blue")
                    .Build();
        }

        public MaskLibrary Generate(Interval frequencyRange, Interval magnitudeRange, int seed, int count)
        {
            var random = new Random(seed);
            var canvasProvider = new CachingCanvasProvider(new(64, 64), Color4.Black);

            var renderTexture = new RenderTexture(new(64, 64));
            var h = 1.05f * 0.5f * s_Sqrt3;
            var verts = new Vertex3[]
            {
                new(new(-0.05f, h, 0f), new(1f, 0f, 0f, 1f), new(0f, 1f)),
                new(new(0.5f, -0.05f, 0f), new(0f, 1f, 0f, 1f), new(0f, 0f)),
                new(new(1.05f, h, 0f), new(0f, 0f, 1f, 1f), new(1f, 1f))
            };
            _maskShader.SetFloat("edge_delta", -0.05f);
            renderTexture.PushProjection(new(-10, Matrix4.CreateOrthographicOffCenter(0, 1, 0, 1, -10, 10)));
            renderTexture.PushViewMatrix(Matrix4.Identity);
            renderTexture.PushModelMatrix(Matrix4.Identity);
            var options = new MaskLibrary.Option[count * s_Distributions.Length];
            var sheet =
                new DynamicStaticSizeTexturePage(
                    new(1024, 1024),
                    new(64, 64),
                    Color4.Black,
                    new(),
                    new()
                    {
                        MinFilter = TextureMinFilter.Nearest,
                        MagFilter = TextureMagFilter.Nearest
                    });
            int option = 0;
            for (int i = 0; i < s_Distributions.Length; ++i)
            {
                _maskShader.SetVector4("mask", s_Distributions[i].Mask + s_Offset);
                for (int j = 0; j < count; ++j)
                {
                    _aSeed.Value = random.Next();
                    _wSeed.Value = random.Next();
                    var freq = random.NextSingle() * (frequencyRange.Maximum - frequencyRange.Minimum)
                        + frequencyRange.Minimum;
                    _frequency.Value = new(freq, freq, freq);

                    var result = _pipeline.Run(canvasProvider)[0];
                    var texture = result.GetTexture();
                    canvasProvider.Return(result);

                    _maskShader.SetFloat(
                        "magnitude",
                        (float)(magnitudeRange.Minimum
                            + random.NextDouble() * (magnitudeRange.Maximum - magnitudeRange.Minimum)));
                    renderTexture.Clear();
                    renderTexture.Draw(
                        verts,
                        PrimitiveType.Triangles,
                        0,
                        verts.Length,
                        new(BlendMode.None, _maskShader, texture));
                    renderTexture.Display();
                    sheet.Add(renderTexture.GetTexture(), out var segment);
                    options[option++] = new(GetTexCoords(segment), s_Distributions[i].Levels);
                }
            }
            canvasProvider.Dispose();
            renderTexture.Dispose();
            return new MaskLibrary(sheet.GetTexture(), options);
        }

        private static Vector2[] GetTexCoords(Box2i segment)
        {
            return new Vector2[]
            {
                segment.Min + 64f * new Vector2(0f, 0.5f * s_Sqrt3),
                segment.Min + 64f * new Vector2(0.5f, 0f),
                segment.Min + 64f * new Vector2(1f, 0.5f * s_Sqrt3)
            };
        }
    }
}
