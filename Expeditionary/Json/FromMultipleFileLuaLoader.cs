using Cardamom;
using Cardamom.Utils.IO;
using NLua;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Expeditionary.Json
{
    public class FromMultipleFileLuaLoader: JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert
                .GetInterfaces()
                .Any(x => x.IsGenericType && typeof(IDictionary<,>).IsAssignableFrom(x.GetGenericTypeDefinition()));
        }

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            foreach (var @interface in typeToConvert.GetInterfaces())
            {
                if (@interface.IsGenericType
                    && typeof(IDictionary<,>).IsAssignableFrom(@interface.GetGenericTypeDefinition()))
                {
                    var args = @interface.GetGenericArguments();
                    Precondition.Check(args[0] == typeof(string));
                    var converterArgs = new Type[] { typeToConvert, args[1] };
                    return (JsonConverter?)Activator.CreateInstance(
                        typeof(FromMultipleFileLuaLoaderImpl<,>).MakeGenericType(converterArgs));
                }
            }
            throw new JsonException();
        }


        class FromMultipleFileLuaLoaderImpl<TCollection, TValue> : JsonConverter<TCollection> 
            where TCollection : IDictionary<string, TValue>
            where TValue : IKeyed
        {
            public override TCollection Read(
                ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var result = (TCollection)Activator.CreateInstance(typeToConvert)!;
                var patterns = JsonSerializer.Deserialize<List<string>>(ref reader, options)!;
                var files = new HashSet<string>(patterns.SelectMany(Glob.GetFiles));
                foreach (var file in files)
                {
                    var lua = new Lua();
                    lua.DoFile(file);
                    foreach (var element in lua.GetFunction("Load").Call()
                        .Cast<LuaTable>().First().Values.Cast<TValue>())
                    {
                        result.Add(element.Key, element);
                        if (options.ReferenceHandler != null)
                        {
                            var resolver = options.ReferenceHandler.CreateResolver();
                            var referenceId = resolver.GetReference(element, out bool exists);
                            if (!exists)
                            {
                                resolver.AddReference(referenceId, element);
                            }
                        }
                    }
                }
                return result;
            }

            public override void Write(Utf8JsonWriter writer, TCollection @object, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }
        }
    }
}
