using Cardamom.Collections;
using Cardamom.Graphics;
using Cardamom.Graphics.TexturePacking;
using Cardamom.Json.Graphics;
using Expeditionary.Model.Combat.Units;
using OpenTK.Mathematics;
using System.Text.Json.Serialization;

namespace Expeditionary.View.Textures.Generation.Combat.Units
{
    public class UnitTextureGeneratorSettings
    {
        public RenderShader? Shader { get; set; }
        public ITextureVolume? Images { get; set; }
        public Vector2i TextureSize { get; set; }
        public Vector2i ElementSize { get; set; }
        [JsonConverter(typeof(FontJsonConverter))]
        public Font? Font { get; set; }
        public string BackgroundImage { get; set; } = string.Empty;
        public string BorderImage { get; set; } = string.Empty;
        public string SizeImage { get; set; } = string.Empty;
        public EnumMap<UnitTag, string> TagImages { get; set; } = new();
    }
}
