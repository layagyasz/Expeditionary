using Cardamom.Graphics.TexturePacking;
using Cardamom.Json.Graphics.TexturePacking;
using Cardamom.Json.OpenTK;
using OpenTK.Mathematics;
using System.Text.Json.Serialization;

namespace Expeditionary.View.Textures.Generation.Combat
{
    public class UnitTextureGeneratorSettings
    {
        [JsonConverter(typeof(TextureVolumeJsonConverter))]
        public ITextureVolume? Components { get; set; }

        [JsonConverter(typeof(Vector2iJsonConverter))]
        public Vector2i TextureSize { get; set; }

        [JsonConverter(typeof(Vector2iJsonConverter))]
        public Vector2i ElementSize { get; set; }
    }
}
