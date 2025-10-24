using Cardamom.Graphics;
using Cardamom.Ui;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Expeditionary.View.Common.Components
{
    public class TextureBackground : IRenderable
    {
        private static readonly Vector2[] s_Points =
        {
            new(0f, 0f),
            new(1f, 0f),
            new(0f, 1f),
            new(0f, 1f),
            new(1f, 0f),
            new(1f, 1f)
        };

        private readonly Texture _texture;
        private readonly Box2i _textureBox;
        private readonly RenderShader _shader;
        private readonly Vertex3[] _vertices;

        private Matrix4 _matrix;

        public TextureBackground(Texture texture, Box2i textureBox, RenderShader shader)
        {
            _texture = texture;
            _textureBox = textureBox;
            _shader = shader;
            _matrix = Matrix4.Identity;
            _vertices = new Vertex3[6];
            for (int i=0; i<s_Points.Length; ++i)
            {
                var p = s_Points[i];
                _vertices[i] = new Vertex3(new(p.X, p.Y, 0), Color4.White, textureBox.Min + textureBox.Size * p);
            }
        }

        public void Draw(IRenderTarget target, IUiContext context)
        {
            target.PushModelMatrix(_matrix);
            target.Draw(
                _vertices,
                PrimitiveType.Triangles,
                0, 
                _vertices.Length,
                new RenderResources(BlendMode.None, _shader, _texture));
            target.PopModelMatrix();
        }

        public void Initialize() { }

        public void ResizeContext(Vector3 context)
        {
            var ratio = context / _textureBox.Size.X;
            var scale = Math.Max(ratio.X, ratio.Y);
            _matrix = Matrix4.CreateScale(scale * context) * Matrix4.CreateTranslation(0.5f * context * (1 - scale));
        }

        public void Update(long delta) { }
    }
}
