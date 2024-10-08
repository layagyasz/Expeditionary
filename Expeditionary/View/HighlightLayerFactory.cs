using Cardamom.Graphics;

namespace Expeditionary.View
{
    public class HighlightLayerFactory
    {
        private readonly RenderShader _shader;

        public HighlightLayerFactory(RenderShader shader)
        {
            _shader = shader;
        }

        public HighlightLayer Create()
        {
            return new(_shader);
        }
    }
}
