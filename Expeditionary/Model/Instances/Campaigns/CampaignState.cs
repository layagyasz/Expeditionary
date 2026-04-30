namespace Expeditionary.Model.Instances.Campaigns
{
    public record class CampaignState(IDictionary<string, int> Stages)
    {
        public static CampaignState Empty()
        {
            return new(new Dictionary<string, int>());
        }

        public int Get(string key, int initialStageId)
        {
            if (Stages.TryGetValue(key, out int stageId))
            {
                return stageId;
            }
            return initialStageId;
        }

        public void Put(string key, int stageId)
        {
            Stages[key] = stageId;
        }
    }
}
