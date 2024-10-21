using Cardamom.Collections;
using Cardamom.Json;
using Cardamom.Utils.IO;
using System.Text.Json.Serialization;

namespace Expeditionary.Scripting
{
    [JsonConverter(typeof(BuilderJsonConverter))]
    [BuilderClass(typeof(Builder))]
    public class LuaScriptLibrary : IDisposable
    {
        public static readonly LuaScriptLibrary Empty = new(new());

        private Library<LuaScript>? _scripts;

        private LuaScriptLibrary(Library<LuaScript> scripts)
        {
            _scripts = scripts;
        }

        public void Dispose()
        {
            foreach (var script in _scripts!.Values)
            {
                script.Dispose();
            }
            _scripts = null;
            GC.SuppressFinalize(this);
            GC.KeepAlive(this);
        }

        public IEnumerable<KeyValuePair<string, LuaScript>> GetScripts()
        {
            return _scripts!;
        }

        public class Builder
        {
            public List<string> Paths { get; set; } = new();

            public LuaScriptLibrary Build()
            {
                var lib = new Library<LuaScript>();
                foreach (var file in Paths.SelectMany(Glob.GetFiles))
                {
                    lib.Add(Path.GetFileNameWithoutExtension(file), LuaScript.FromFile(file));
                }
                return new(lib);
            }
        }
    }
}
