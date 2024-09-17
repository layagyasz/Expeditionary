using Cardamom.Collections;
using Cardamom.Json.Collections;
using Expeditionary.Model.Combat;
using System.Text.Json.Serialization;

namespace Expeditionary.Model
{
    public class GameModule
    {
        [JsonConverter(typeof(FromMultipleFileJsonConverter))]
        [JsonPropertyOrder(0)]
        public Library<UnitTrait> UnitTraits { get; set; } = new();

        [JsonConverter(typeof(FromMultipleFileJsonConverter))]
        [JsonPropertyOrder(1)]
        public Library<UnitDefinition> UnitDefinitions { get; set; } = new();
    }
}
