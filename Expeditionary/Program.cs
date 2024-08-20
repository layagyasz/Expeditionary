using Cardamom.Graphics;
using Cardamom.Graphics.TexturePacking;
using Cardamom.ImageProcessing;
using Cardamom.ImageProcessing.Pipelines;
using Cardamom.ImageProcessing.Pipelines.Nodes;
using Cardamom.Ui;
using Cardamom.Utils.Suppliers;
using Cardamom.Window;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Expeditionary
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var window = new RenderWindow("Expeditionary", new(512, 512));
            var ui = new UiWindow(window);
            ui.Bind(new MouseListener());
            ui.Bind(new KeyboardListener(SimpleKeyMapper.Us, Array.Empty<Keys>()));

            var random = new Random();
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
            var shader = new RenderShader.Builder().SetVertex("default.vert").SetFragment("blend.frag").Build();
            shader.SetFloat("attenuation", 2f);
            shader.SetColor("color_a", new Color4(1f, 0f, 0f, 1f));
            shader.SetColor("color_b", new Color4(0f, 1f, 0f, 1f));
            shader.SetColor("color_c", new Color4(0f, 0f, 1f, 1f));
            renderTexture.PushProjection(new(-10, Matrix4.CreateOrthographicOffCenter(0, 1, 1, 0, -10, 10)));
            renderTexture.PushViewMatrix(Matrix4.Identity);
            renderTexture.PushModelMatrix(Matrix4.Identity);
            var sheet = 
                new DynamicTextureVolume(
                    new DynamicStaticSizeTexturePage.Supplier(
                        new(1024, 1024), new(64, 64), Color4.Black, new()), /* checkAllPages= */ false);
            for (int i = 0; i < 100; ++i)
            {
                var result = noise.Run(canvasProvider)[0];
                var texture = result.GetTexture();
                canvasProvider.Return(result);
                renderTexture.Clear();
                renderTexture.Draw(
                    verts,
                    PrimitiveType.Triangles,
                    0,
                    verts.Length,
                    new(BlendMode.None, shader, texture));
                renderTexture.Display();
                sheet.Add(string.Format("tile-base-{0}", i), renderTexture.CopyTexture());
            }
            foreach (var texture in sheet.GetTextures())
            {
                texture.CopyToImage().SaveToFile(string.Format("{0}.png", texture.GetHashCode()));
            }
        }
    }
}
