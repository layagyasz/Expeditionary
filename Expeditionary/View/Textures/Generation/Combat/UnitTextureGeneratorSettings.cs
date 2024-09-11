using Cardamom.Collections;
using Cardamom.Graphics.TexturePacking;
using Expeditionary.Model.Combat;
using OpenTK.Mathematics;

namespace Expeditionary.View.Textures.Generation.Combat
{
    public class UnitTextureGeneratorSettings
    {
        public ITextureVolume? Components { get; set; }
        public Vector2i TextureSize { get; set; }
        public Vector2i ElementSize { get; set; }
        public EnumMap<UnitTag, string> TagMapping { get; set; } = new();
    }
}
