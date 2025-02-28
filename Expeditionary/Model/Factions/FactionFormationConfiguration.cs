using Cardamom;
using Cardamom.Json.Collections;
using Expeditionary.Model.Formations;
using Expeditionary.Model.Formations.Generator;
using System.Text.Json.Serialization;

namespace Expeditionary.Model.Factions
{
    public class FactionFormationConfiguration : IKeyed
    {
        public string Key { get; set; } = string.Empty;
        public string Faction { get; set; } = string.Empty;
        public List<UnitUsage> Units { get; set; } = new();

        [JsonConverter(typeof(ReferenceCollectionJsonConverter))]
        public List<FormationGenerator> Formations { get; set; } = new();
    }
}
