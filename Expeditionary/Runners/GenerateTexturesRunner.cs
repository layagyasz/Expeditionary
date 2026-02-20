using Cardamom.Json.Graphics;
using Cardamom.Json.OpenTK;
using Cardamom.Ui;
using Expeditionary.Loader;
using Expeditionary.View.Screens;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Expeditionary.Runners
{
    public class GenerateTexturesRunner : UiRunner
    {
        public GenerateTexturesRunner(ProgramConfig config)
            : base(config) { }

        protected override void Handle(
            ProgramData data, UiWindow window, ThreadedLoader loader, ScreenFactory screenFactory)
        {
            JsonSerializerOptions options = new()
            {
                NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
                WriteIndented = true,
            };
            options.Converters.Add(new TextureJsonConverter());
            options.Converters.Add(new Vector2JsonConverter());
            options.Converters.Add(new Vector2iJsonConverter());
            options.Converters.Add(new Vector3iJsonConverter());

            using FileStream fileStream = new("texture_library.json", FileMode.Create, FileAccess.Write);
            JsonSerializer.Serialize(fileStream, data.TextureLibrary, options);
        }
    }
}
