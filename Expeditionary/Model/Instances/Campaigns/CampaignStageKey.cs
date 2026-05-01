namespace Expeditionary.Model.Instances.Campaigns
{
    public record struct CampaignStageKey(string CampaignKey, int StageId)
    {
        public bool CampaignEquals(CampaignStageKey other)
        {
            return other.CampaignKey == CampaignKey;
        }
    }
}
