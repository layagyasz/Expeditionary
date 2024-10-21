using Cardamom;
using Expeditionary.Scripting;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Expeditionary.Json
{
    public class LuaScriptLibraryConverter : JsonConverter<LuaScriptLibrary>
    {
        public override LuaScriptLibrary Read(
            ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var scripts = JsonSerializer.Deserialize<LuaScriptLibrary.Builder>(ref reader, options)!.Build();
            if (options.ReferenceHandler != null)
            {
                var resolver = options.ReferenceHandler.CreateResolver();
                foreach (var script in scripts.GetScripts())
                {
                    var wrapper = new KeyedWrapper<LuaScript>() { Key = script.Key, Element = script.Value };
                    var referenceId = resolver.GetReference(wrapper, out bool exists);
                    if (!exists)
                    {
                        resolver.AddReference(referenceId, wrapper.Element);
                    }
                }
            }
            return scripts;
        }

        public override void Write(Utf8JsonWriter writer, LuaScriptLibrary @object, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
