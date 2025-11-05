using Cardamom;
using Cardamom.Json.Collections;
using System.Text.Json.Serialization;

namespace Expeditionary.Model.Mapping.Environments
{
    public class MapEnvironmentDefinition : IKeyed
    {
        public string Key { get; set; } = string.Empty;
        public MapEnvironmentName Name { get; set; } = new();

        [JsonConverter(typeof(ReferenceCollectionJsonConverter))]
        public List<MapEnvironmentTrait> Traits { get; set; } = new();

        public MapEnvironment GetEnvironment()
        {
            var env = new MapEnvironment(Name);
            foreach (var trait in Traits)
            {
                trait.Apply!.Call(trait, env);
            }
            return env;
        }
    }
}
