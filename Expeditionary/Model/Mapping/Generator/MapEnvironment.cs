using Cardamom;
using Cardamom.Json.Collections;
using System.Text.Json.Serialization;

namespace Expeditionary.Model.Mapping.Generator
{
    public class MapEnvironment : IKeyed
    {
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        [JsonConverter(typeof(ReferenceCollectionJsonConverter))]
        public List<MapEnvironmentModifier> Modifiers { get; set; } = new();

        public MapGenerator.Parameters GetParameters()
        {
            var parameters = new MapGenerator.Parameters();
            foreach (var modifier in Modifiers)
            {
                modifier.Apply!.Call(modifier, parameters);
            }
            return parameters;
        }
    }
}
