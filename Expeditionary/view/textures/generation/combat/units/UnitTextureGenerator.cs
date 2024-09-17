using Cardamom.Graphics.TexturePacking;
using Expeditionary.Model.Combat.Units;

namespace Expeditionary.View.Textures.Generation.Combat.Units
{
    public class UnitTextureGenerator
    {
        private UnitTextureGeneratorSettings _settings;

        public UnitTextureGenerator(UnitTextureGeneratorSettings settings)
        {
            _settings = settings;
        }

        public ITextureVolume Generate(IEnumerable<UnitType> units)
        {
            return new DynamicTextureVolume(
                new DynamicStaticSizeTexturePage.Supplier(
                    _settings.TextureSize, _settings.ElementSize, new(), new(), new()),
                checkAllPages: false);
        }
    }
}
