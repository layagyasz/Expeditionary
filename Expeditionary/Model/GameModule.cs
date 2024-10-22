using Cardamom.Collections;
using Cardamom.Json.Collections;
using Expeditionary.Json;
using Expeditionary.Model.Combat.Units;
using Expeditionary.Model.Factions;
using Expeditionary.Model.Mapping.Generator;
using System.Text.Json.Serialization;

namespace Expeditionary.Model
{
    public class GameModule
    {
        [JsonConverter(typeof(FromMultipleFileLuaLoader))]
        public Library<MapEnvironmentModifier> MapEnvironmentModifiers { get; set; } = new();

        [JsonConverter(typeof(FromMultipleFileJsonConverter))]
        public Library<MapEnvironment> Environments { get; set; } = new();

        [JsonConverter(typeof(FromMultipleFileJsonConverter))]
        public Library<UnitTrait> UnitTraits { get; set; } = new();

        [JsonConverter(typeof(FromMultipleFileJsonConverter))]
        public Library<UnitType> UnitTypes { get; set; } = new();

        [JsonConverter(typeof(FromMultipleFileJsonConverter))]
        public Library<Faction> Factions { get; set; } = new();
    }
}
