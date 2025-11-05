using Cardamom.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Expeditionary.View.Scenes.Galaxies
{
    public class GalaxyViewFactory
    {
        private static readonly Vector2i s_LookupSize = new(512, 512);
        private static readonly int s_TransformLocation = 0;

        private readonly ComputeShader _lookupShader;
        private readonly RenderShader _shader;

        public GalaxyViewFactory(ComputeShader lookupShader, RenderShader shader)
        {
            _lookupShader = lookupShader;
            _shader = shader;
        }

        public GalaxyView Create()
        {
            var transform = 
                Matrix4.CreateScale(2f / s_LookupSize.X, 2f / s_LookupSize.Y, 1f)
                * Matrix4.CreateTranslation(-1, -1, 0);
            _lookupShader.SetMatrix4(s_TransformLocation, transform);

            var p = new Texture.Parameters() { WrapMode = TextureWrapMode.ClampToEdge };
            var tex = Texture.Create(s_LookupSize, p);
            tex.BindImage(0);
            _lookupShader.DoCompute(s_LookupSize);
            Texture.UnbindImage(0);

            return new(tex, Texture.FromFile("resources/view/textures/luts/lut_galaxy.png", p), _shader);
        }
    }
}
