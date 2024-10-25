using Cardamom;
using Cardamom.Json.Collections;
using System.Text.Json.Serialization;

namespace Expeditionary.Model.Mapping
{
    public class MapEnvironmentDefinition : IKeyed
    {
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        [JsonConverter(typeof(ReferenceCollectionJsonConverter))]
        public List<MapEnvironmentModifier> Modifiers { get; set; } = new();

        public MapEnvironment GetEnvironment()
        {
            var env = new MapEnvironment();
            foreach (var modifier in Modifiers)
            {
                modifier.Apply!.Call(modifier, env);
            }
            return env;
        }
    }
}
