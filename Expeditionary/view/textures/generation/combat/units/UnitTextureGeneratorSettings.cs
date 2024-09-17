using Cardamom.Collections;
using Cardamom.Graphics.TexturePacking;
using Expeditionary.model.combat.units;
using OpenTK.Mathematics;

namespace Expeditionary.view.textures.generation.combat.units
{
    public class UnitTextureGeneratorSettings
    {
        public ITextureVolume? Components { get; set; }
        public Vector2i TextureSize { get; set; }
        public Vector2i ElementSize { get; set; }
        public EnumMap<UnitTag, string> TagMapping { get; set; } = new();
    }
}
