using Cardamom.Graphics;
using Cardamom;
using OpenTK.Mathematics;
using Cardamom.Ui;
using OpenTK.Graphics.OpenGL4;

namespace Expeditionary.View.Scenes.Galaxies
{
    public class GalaxyView : ManagedResource, IRenderable
    {
        private static readonly float s_Distortion = 0.000001f;
        private static readonly Vertex3[] s_Vertices =
        {
            new(new(-2f, 0f, -2f), Color4.White, new(-1f, -1f)),
            new(new(2f, 0f, -2f), Color4.White, new(2f, -1f)),
            new(new(-2f, 0f, 2f), Color4.White, new(-1f, 2f)),
            new(new(-2f, 0f, 2f), Color4.White, new(-1f, 2f)),
            new(new(2f, 0f, -2f), Color4.White, new(2f, -1f)),
            new(new(2f, 0f, 2f), Color4.White, new(2f, 2f))
        };

        private readonly Texture _lookup;
        private readonly RenderShader _shader;

        private long _time;

        public GalaxyView(Texture lookup, RenderShader shader)
        {
            _lookup = lookup;
            _shader = shader;
        }

        protected override void DisposeImpl() 
        {
            _lookup.Dispose();
        }

        public void Draw(IRenderTarget target, IUiContext context)
        {
            _shader.SetFloat("time", s_Distortion * _time);
            target.Draw(
                s_Vertices, PrimitiveType.Triangles, 0, s_Vertices.Length, new(BlendMode.Alpha, _shader, _lookup));
        }

        public void Initialize() { }

        public void ResizeContext(Vector3 context) { }

        public void Update(long delta) 
        {
            _time += delta;
        }
    }
}
