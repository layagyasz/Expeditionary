using Cardamom;
using Cardamom.Json.Collections;
using Expeditionary.Model.Factions;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Expeditionary.Model.Campaigns
{
    public record class Campaign : IKeyed
    {
        public required string Key { get; set; }
        public required string Name { get; set; }

        [JsonConverter(typeof(ReferenceCollectionJsonConverter))]
        public required List<Faction> Factions { get; set; }

        public required int InitialNodeId { get; set; }
        public required ImmutableList<CampaignNode> Nodes { get; set; }
    }
}
