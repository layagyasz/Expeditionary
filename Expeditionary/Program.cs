using Cardamom.Graphics;
using Cardamom.Ui;
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

            var renderTexture = new RenderTexture(new(128, 128));
            var verts = new Vertex3[]
            {
                new(new(0.5f, 0f, 0f), new(1f, 0f, 0f, 1f), new()),
                new(new(1f, 1f, 0f), new(0f, 1f, 0f, 1f), new()),
                new(new(0f, 1f, 0f), new(0f, 0f, 1f, 1f), new())
            };
            renderTexture.PushProjection(new(-10, Matrix4.CreateOrthographicOffCenter(0, 1, 1, 0, -10, 10)));
            renderTexture.PushViewMatrix(Matrix4.Identity);
            renderTexture.PushModelMatrix(Matrix4.Identity);
            renderTexture.Clear();
            renderTexture.Draw(
                verts,
                PrimitiveType.Triangles,
                0,
                verts.Length,
                new(
                    BlendMode.None,
                    new RenderShader.Builder().SetVertex("default.vert").SetFragment("default_no_tex.frag").Build()));
            renderTexture.Display();
            renderTexture.CopyToImage().SaveToFile("output.png");
        }
    }
}
