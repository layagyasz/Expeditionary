namespace Expeditionary.Model.Instances.Campaigns
{
    public class CampaignState
    {
        private readonly Dictionary<CampaignStageKey, CampaignStageState> _stages;

        private CampaignState(Dictionary<CampaignStageKey, CampaignStageState> stages)
        {
            _stages = stages;
        }

        public static CampaignState Empty()
        {
            return new(new());
        }

        public IEnumerable<CampaignStageKey> GetActive()
        {
            return _stages.Where(kvp => kvp.Value == CampaignStageState.Open).Select(kvp => kvp.Key);
        }

        public IEnumerable<CampaignStageKey> GetDormant()
        {
            return _stages.Where(kvp => kvp.Value == CampaignStageState.Dormant).Select(kvp => kvp.Key);
        }

        public CampaignStageState Get(CampaignStageKey stageKey)
        {
            if (_stages.TryGetValue(stageKey, out CampaignStageState state))
            {
                return state;
            }
            return CampaignStageState.Dormant;
        }

        public void Put(CampaignStageKey stageKey, CampaignStageState state)
        {
            if (state == CampaignStageState.Dormant)
            {
                _stages.Remove(stageKey);
            }
            else
            {
                _stages[stageKey] = state;
            }
        }
    }
}
