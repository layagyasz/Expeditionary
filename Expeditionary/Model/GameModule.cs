using Cardamom.Collections;
using Cardamom.Json.Collections;
using Expeditionary.Model.Combat.Units;
using Expeditionary.Model.Factions;
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
        public Library<UnitType> UnitTypes { get; set; } = new();

        [JsonConverter(typeof(FromMultipleFileJsonConverter))]
        [JsonPropertyOrder(2)]
        public Library<Faction> Factions { get; set; } = new();
    }
}
