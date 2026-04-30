using Expeditionary.Model.Missions.Generator;
using System.Collections.Immutable;

namespace Expeditionary.Model.Instances.Campaigns
{
    public record class CampaignStage(
        int Id, ImmutableList<CampaignTransition> Transitions, ImmutableList<MissionNode> MissionNodes)
    {
        public int? TryTransition(Campaign campaign, GameInstance instance)
        {
            return Transitions.First(
                transition => transition.Trigger.Trigger(campaign, this, instance))?.TargetStageId;
        }
    }
}
