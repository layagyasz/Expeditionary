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

        public required int InitialStageId { get; set; }
        public required ImmutableList<CampaignStage> Stages { get; set; }

        public CampaignStage Get(CampaignState state)
        {
            return Get(state.Get(Key, InitialStageId));
        }

        public void Step(CampaignState state, GameInstance instance)
        {
            var currentStageId = state.Get(Key, InitialStageId);
            var currentStage = Get(state);
            int? nextStageId = currentStageId;
            do
            {
                currentStageId = nextStageId.Value;
                nextStageId = currentStage.TryTransition(this, instance);
            }
            while (nextStageId != null);
            state.Put(Key, currentStageId);
        }

        private CampaignStage Get(int stageId)
        {
            return Stages.First(stage => stage.Id == stageId);
        }
    }
}
