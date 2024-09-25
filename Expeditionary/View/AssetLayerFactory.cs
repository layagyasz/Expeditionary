using Cardamom.Graphics;
using Cardamom.Graphics.TexturePacking;

namespace Expeditionary.View
{
    public class AssetLayerFactory
    {
        private readonly RenderShader _shader;
        private readonly ITextureVolume _textures;

        public AssetLayerFactory(RenderShader shader, ITextureVolume texture)
        {
            _shader = shader;
            _textures = texture;
        }

        public AssetLayer Create()
        {
            return new(_shader, _textures);
        }
    }
}
