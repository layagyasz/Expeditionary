using Cardamom.Collections;
using Cardamom.Json;
using Cardamom.Json.Collections;
using Expeditionary.Json;
using Expeditionary.Model.Campaigns;
using Expeditionary.Model.Factions;
using Expeditionary.Model.Formations.Generator;
using Expeditionary.Model.Galaxies;
using Expeditionary.Model.Mapping.Environments;
using Expeditionary.Model.Mapping.Environments.Generator;
using Expeditionary.Model.Units;
using System.Text.Json.Serialization;

namespace Expeditionary.Model
{
    public class GameModule
    {
        [JsonPropertyOrder(11)]
        [JsonConverter(typeof(FromMultipleFileJsonConverter))]
        public required Library<Campaign> Campaigns { get; set; }

        [JsonPropertyOrder(1)]
        [JsonConverter(typeof(FromMultipleFileJsonConverter))]
        public required Library<MapEnvironmentDefinition> Environments { get; set; }

        [JsonPropertyOrder(6)]
        [JsonConverter(typeof(FromMultipleFileJsonConverter))]
        public required Library<Faction> Factions { get; set; }

        [JsonPropertyOrder(7)]
        [JsonConverter(typeof(FromMultipleFileJsonConverter))]
        public required Library<FactionFormationConfiguration> FactionFormations { get; set; }

        [JsonPropertyOrder(5)]
        [JsonConverter(typeof(FromMultipleFileJsonConverter))]
        public required Library<FormationTemplateGenerator> Formations { get; set; }

        [JsonPropertyOrder(8)]
        [JsonConverter(typeof(FromFileJsonConverter))]
        public required Galaxy Galaxy { get; set; }

        [JsonPropertyOrder(10)]
        [JsonConverter(typeof(FromFileJsonConverter))]
        public required MapEnvironmentGenerator MapEnvironmentGenerator { get; set; }

        [JsonPropertyOrder(0)]
        [JsonConverter(typeof(FromMultipleFileLuaLoader))]
        public required Library<MapEnvironmentTrait> MapEnvironmentTraits { get; set; }

        [JsonPropertyOrder(9)]
        [JsonConverter(typeof(FromMultipleFileJsonConverter))]
        public required Library<SectorNaming> SectorNamings { get; set; }

        [JsonPropertyOrder(2)]
        [JsonConverter(typeof(FromMultipleFileJsonConverter))]
        public required Library<UnitTrait> UnitTraits { get; set; }

        [JsonPropertyOrder(4)]
        [JsonConverter(typeof(FromMultipleFileJsonConverter))]
        public required Library<UnitType> UnitTypes { get; set; }

        [JsonPropertyOrder(3)]
        [JsonConverter(typeof(FromMultipleFileJsonConverter))]
        public required Library<UnitWeaponDefinition> UnitWeapons { get; set; }
    }
}
