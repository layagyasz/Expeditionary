using Cardamom.Collections;
using Cardamom.Json;
using Cardamom.Json.Collections;
using Expeditionary.Json;
using Expeditionary.Model.Factions;
using Expeditionary.Model.Formations.Generator;
using Expeditionary.Model.Mapping.Environments;
using Expeditionary.Model.Mapping.Environments.Generator;
using Expeditionary.Model.Sectors;
using Expeditionary.Model.Units;
using System.Text.Json.Serialization;

namespace Expeditionary.Model
{
    public class GameModule
    {
        [JsonConverter(typeof(FromMultipleFileLuaLoader))]
        public Library<MapEnvironmentTrait> MapEnvironmentTraits { get; set; } = new();

        [JsonConverter(typeof(FromMultipleFileJsonConverter))]
        public Library<MapEnvironmentDefinition> Environments { get; set; } = new();

        [JsonConverter(typeof(FromMultipleFileJsonConverter))]
        public Library<UnitTrait> UnitTraits { get; set; } = new();

        [JsonConverter(typeof(FromMultipleFileJsonConverter))]
        public Library<UnitWeaponDefinition> UnitWeapons { get; set; } = new();

        [JsonConverter(typeof(FromMultipleFileJsonConverter))]
        public Library<UnitType> UnitTypes { get; set; } = new();

        [JsonConverter(typeof(FromMultipleFileJsonConverter))]
        public Library<FormationTemplateGenerator> Formations { get; set; } = new();

        [JsonConverter(typeof(FromMultipleFileJsonConverter))]
        public Library<Faction> Factions { get; set; } = new();

        [JsonConverter(typeof(FromMultipleFileJsonConverter))]
        public Library<FactionFormationConfiguration> FactionFormations { get; set; } = new();

        [JsonConverter(typeof(FromMultipleFileJsonConverter))]
        public Library<SectorNaming> SectorNamings { get; set; } = new();

        [JsonConverter(typeof(FromFileJsonConverter))]
        public MapEnvironmentGenerator MapEnvironmentGenerator { get; set; } = new();
    }
}
