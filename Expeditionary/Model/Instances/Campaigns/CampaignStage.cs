using System.Collections.Immutable;

namespace Expeditionary.Model.Instances.Campaigns
{
    public record class CampaignStage
    {
        public required int Id { get; init; }
        public ICampaignTrigger OpenTrigger { get; init; } = new ICampaignTrigger.AlwaysTrigger();
        public ICampaignTrigger CloseTrigger { get; init; } = new ICampaignTrigger.NeverTrigger();
        public required ImmutableList<MissionNode> MissionNodes { get; init; }
    }
}
