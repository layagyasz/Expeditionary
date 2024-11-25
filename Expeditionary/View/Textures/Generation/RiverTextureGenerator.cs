﻿using Cardamom.Graphics.TexturePacking;
using Cardamom.Graphics;
using Cardamom.ImageProcessing.Pipelines.Nodes;
using Cardamom.ImageProcessing.Pipelines;
using Cardamom.Mathematics;
using Cardamom.Utils.Suppliers;
using OpenTK.Mathematics;
using Cardamom.ImageProcessing;
using OpenTK.Graphics.OpenGL4;

namespace Expeditionary.View.Textures.Generation
{
    public class RiverTextureGenerator
    {
        private static readonly float s_Sqrt3 = MathF.Sqrt(3);
        private static readonly int[] s_Masks = { 1, 3, 7 };

        private readonly RenderShader _riverShader;

        private readonly Pipeline _pipeline;
        private readonly ConstantSupplier<int> _aSeed = new();
        private readonly ConstantSupplier<int> _bSeed = new();
        private readonly ConstantSupplier<int> _wSeed = new();
        private readonly ConstantSupplier<Vector3> _frequency = new();

        public RiverTextureGenerator(RenderShader riverShader)
        {
            _riverShader = riverShader;
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
                    .AddNode(
                        new LatticeNoiseNode.Builder()
                            .SetKey("green")
                            .SetInput("input", "gradient")
                            .SetOutput("red")
                            .SetChannel(Channel.Green)
                            .SetParameters(
                                new()
                                {
                                    Seed = _bSeed,
                                    Frequency = _frequency
                                }))
                    .AddNode(
                        new WhiteNoiseNode.Builder()
                            .SetKey("blue")
                            .SetInput("input", "gradient")
                            .SetOutput("green")
                            .SetChannel(Channel.Blue)
                            .SetParameters(
                                new()
                                {
                                    Seed = _wSeed
                                }))
                    .AddOutput("blue")
                    .Build();
        }

        public EdgeLibrary Generate(Interval frequencyRange, Interval magnitudeRange, int seed, int count)
        {  
            var canvasProvider = new CachingCanvasProvider(new(64, 64), Color4.Black);

            var renderTexture = new RenderTexture(new(64, 64));
            var h = 1.05f * 0.5f * s_Sqrt3;
            var verts = new Vertex3[]
            {
                new(new(-0.05f, h, 0f), new(1f, 0f, 0f, 1f), new(0f, 1f)),
                new(new(0.5f, -0.05f, 0f), new(0f, 1f, 0f, 1f), new(0f, 0f)),
                new(new(1.05f, h, 0f), new(0f, 0f, 1f, 1f), new(1f, 1f))
            };
            _riverShader.SetFloat("edge_delta", -0.05f);
            renderTexture.PushProjection(new(-10, Matrix4.CreateOrthographicOffCenter(0, 1, 0, 1, -10, 10)));
            renderTexture.PushViewMatrix(Matrix4.Identity);
            renderTexture.PushModelMatrix(Matrix4.Identity);
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
            var options = new EdgeLibrary.Option[3 * count];
            var random = new Random(seed);
            for (int i = 0; i < count; ++i)
            {
                _aSeed.Value = random.Next();
                _bSeed.Value = random.Next();
                _wSeed.Value = random.Next();
                var freq = frequencyRange.Minimum 
                    + random.NextSingle() * (frequencyRange.Maximum - frequencyRange.Minimum);
                _frequency.Value = new(freq, freq, freq);

                var result = _pipeline.Run(canvasProvider)[0];
                var texture = result.GetTexture();
                canvasProvider.Return(result);
                _riverShader.SetFloat(
                    "magnitude",
                    (float)(magnitudeRange.Minimum
                        + random.NextDouble() * (magnitudeRange.Maximum - magnitudeRange.Minimum)));
                for (int j = 0; j < s_Masks.Length; ++j)
                {
                    _riverShader.SetInt32("mask", s_Masks[j]);
                    renderTexture.Clear();
                    renderTexture.Draw(
                        verts,
                        PrimitiveType.Triangles,
                        0,
                        verts.Length,
                        new(BlendMode.None, _riverShader, texture));
                    renderTexture.Display();
                    sheet.Add(renderTexture.GetTexture(), out var segment);
                    options[3 * i + j] =
                        new EdgeLibrary.Option(
                            GetTexCoords(segment), 
                            new bool[] { (s_Masks[j] & 1) > 0, (s_Masks[j] & 4) > 0, (s_Masks[j] & 2) > 0 });
                }
            }
            canvasProvider.Dispose();
            renderTexture.Dispose();
            return new(sheet, options);
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
