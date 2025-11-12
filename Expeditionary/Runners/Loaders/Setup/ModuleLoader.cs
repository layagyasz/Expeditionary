using Cardamom.Json.Graphics.TexturePacking;
using Cardamom.Json.OpenTK;
using Cardamom.Json;
using Expeditionary.Model;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Expeditionary.Runners.Loaders
{
    public class ModuleLoader
    {
        private readonly string _path;

        public ModuleLoader(string path)
        {
           _path = path;
        }

        public GameModule Load()
        {
            JsonSerializerOptions options = new()
            {
                NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
                ReadCommentHandling = JsonCommentHandling.Skip,
                ReferenceHandler = new KeyedReferenceHandler()
            };
            options.Converters.Add(new ColorJsonConverter());
            options.Converters.Add(new Vector2JsonConverter());
            options.Converters.Add(new Vector2iJsonConverter());
            options.Converters.Add(new TextureVolumeJsonConverter());
            options.Converters.Add(new BuilderJsonConverter());
            return JsonSerializer.Deserialize<GameModule>(File.ReadAllText(_path), options)!;
        }
    }
}
