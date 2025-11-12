using Cardamom.Graphics.TexturePacking;
using Cardamom.Json.Graphics.TexturePacking;
using Cardamom.Json.OpenTK;
using Cardamom.Json;
using Expeditionary.Model;
using Expeditionary.View.Textures.Generation.Combat.Units;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Expeditionary.Runners.Loaders
{
    public class RuntimeUnitTextureLoader
    {
        private readonly GameModule _module;
        private readonly string _settingsPath;

        public RuntimeUnitTextureLoader(GameModule module, string settingsPath)
        {
            _module = module;
            _settingsPath = settingsPath;
        }

        public ITextureVolume Load()
        {
            JsonSerializerOptions options = new()
            {
                NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
                ReadCommentHandling = JsonCommentHandling.Skip,
                ReferenceHandler = new KeyedReferenceHandler()
            };
            options.Converters.Add(new ColorJsonConverter());
            options.Converters.Add(new Vector2iJsonConverter());
            options.Converters.Add(new TextureVolumeJsonConverter());
            options.Converters.Add(new BuilderJsonConverter());
            var unitTextureGeneratorSettings = 
                JsonSerializer.Deserialize<UnitTextureGeneratorSettings>(File.ReadAllText(_settingsPath), options)!;
            var unitTextureGenerator = new UnitTextureGenerator(unitTextureGeneratorSettings);
            return unitTextureGenerator.Generate(_module.UnitTypes.Values);
        }
    }
}
