namespace Expeditionary.Model.Instances.Campaigns
{
    public record class CampaignTransition(ICampaignTrigger Trigger, int TargetStageId);
}
