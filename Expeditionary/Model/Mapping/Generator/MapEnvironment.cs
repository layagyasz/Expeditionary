using Cardamom;
using Cardamom.Json.Collections;
using Expeditionary.Scripting;
using System.Text.Json.Serialization;

namespace Expeditionary.Model.Mapping.Generator
{
    public class MapEnvironment : IKeyed
    {
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        [JsonConverter(typeof(ReferenceCollectionJsonConverter))]
        public List<LuaScript> Modifiers { get; set; } = new();

        public MapGenerator.Parameters GetParameters()
        {
            var parameters = new MapGenerator.Parameters();
            foreach (var script in Modifiers)
            {
                script.Get("MutateMapParameters").Call(parameters);
            }
            return parameters;
        }
    }
}
