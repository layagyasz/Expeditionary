using Cardamom.Graphics;
using Cardamom;
using OpenTK.Mathematics;
using Cardamom.Ui;
using OpenTK.Graphics.OpenGL4;

namespace Expeditionary.View.Scenes.Galaxies
{
    public class GalaxyView : ManagedResource, IRenderable
    {
        private static readonly Vertex3[] s_Vertices =
        {
            new(new(-1f, 0f, -1f), Color4.White, new(-1f, -1f)),
            new(new(1f, 0f, -1f), Color4.White, new(1f, -1f)),
            new(new(-1f, 0f, 1f), Color4.White, new(-1f, 1f)),
            new(new(-1f, 0f, 1f), Color4.White, new(-1f, 1f)),
            new(new(1f, 0f, -1f), Color4.White, new(1f, -1f)),
            new(new(1f, 0f, 1f), Color4.White, new(1f, 1f))
        };

        private readonly RenderShader _shader;

        public GalaxyView(RenderShader shader)
        {
            _shader = shader;
        }

        protected override void DisposeImpl() { }

        public void Draw(IRenderTarget target, IUiContext context)
        {
            target.Draw(s_Vertices, PrimitiveType.Triangles, 0, s_Vertices.Length, new(BlendMode.Alpha, _shader));
        }

        public void Initialize() { }

        public void ResizeContext(Vector3 context) { }

        public void Update(long delta) { }
    }
}
