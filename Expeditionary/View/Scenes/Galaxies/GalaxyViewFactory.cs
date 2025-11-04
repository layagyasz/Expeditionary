using Cardamom.Graphics;

namespace Expeditionary.View.Scenes.Galaxies
{
    public class GalaxyViewFactory
    {
        private readonly RenderShader _shader;

        public GalaxyViewFactory(RenderShader shader)
        {
            _shader = shader;
        }

        public GalaxyView Create()
        {
            return new(_shader);
        }
    }
}
