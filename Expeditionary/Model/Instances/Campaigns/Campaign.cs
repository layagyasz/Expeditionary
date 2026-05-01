using Cardamom;
using Cardamom.Json.Collections;
using Expeditionary.Model.Factions;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Expeditionary.Model.Instances.Campaigns
{
    public record class Campaign : IKeyed
    {
        public required string Key { get; set; }
        public required string Name { get; set; }

        [JsonConverter(typeof(ReferenceCollectionJsonConverter))]
        public required List<Faction> Factions { get; set; }

        public required ImmutableList<CampaignStage> Stages { get; set; }

        public CampaignStage Get(int stageId)
        {
            return Stages.First(stage => stage.Id == stageId);
        }

        private IEnumerable<CampaignStage> GetAll(IEnumerable<int> stageIds)
        {
            return stageIds.Select(stageId => Stages.First(stage => stage.Id == stageId));
        }
    }
}
