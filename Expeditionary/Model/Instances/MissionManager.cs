using Expeditionary.Model.Factions;
using Expeditionary.Model.Instances.Campaigns;

namespace Expeditionary.Model.Instances
{
    public class MissionManager
    {
        public EventHandler<InstanceMission>? MissionAdded { get; set; }
        public EventHandler<InstanceMission>? MissionRemoved { get; set; }

        public IEnumerable<InstanceMission> Missions => _missions;

        private readonly HashSet<Faction> _factions;
        private readonly MissionGenerator _missionGenerator;
        private readonly Dictionary<string, Campaign> _relevantCampaigns;
        private readonly Dictionary<CampaignStageKey, CampaignStage> _relevantStages;
        private readonly Random _random;

        private long _time;
        private List<InstanceMission> _missions;
        private readonly CampaignState _campaignState;

        public MissionManager(
            IEnumerable<Faction> factions, 
            MissionGenerator missionGenerator,
            IEnumerable<Campaign> campaigns,
            CampaignState campaignState,
            Random random)
        {
            _factions = factions.ToHashSet();
            _missionGenerator = missionGenerator;
            _relevantCampaigns = 
                campaigns
                    .Where(campaign => campaign.Factions.Intersect(_factions).Any())
                    .ToDictionary(campaign => campaign.Key, campaign => campaign);
            _relevantStages = 
                new Dictionary<CampaignStageKey, CampaignStage>(
                    _relevantCampaigns.SelectMany(
                        campaign => campaign.Value.Stages.Select(
                            stage => new KeyValuePair<CampaignStageKey, CampaignStage>(
                                new(campaign.Key, stage.Id), stage))));
            _campaignState = campaignState;
            _random = random;
            _missions = new();
        }

        public void StepCampaigns(GameInstance instance)
        {
            foreach ((var stageKey, var stage) in _relevantStages)
            {
                var state = _campaignState.Get(stageKey);
                if (state == CampaignStageState.Dormant)
                {
                    if (stage.OpenTrigger.Trigger(stageKey, instance))
                    {
                        _campaignState.Put(stageKey, CampaignStageState.Open);
                    }
                } 
                else if (state == CampaignStageState.Open)
                {
                    if (stage.CloseTrigger.Trigger(stageKey, instance))
                    {
                        _campaignState.Put(stageKey, CampaignStageState.Closed);
                    }
                }
            }
        }

        public void StepMissions()
        {
            _time++;
            var newMissions = new List<InstanceMission>();
            var removedMissions = new List<InstanceMission>();
            var addedMissions = new List<InstanceMission>();
            foreach (var mission in _missions)
            {
                if (_time < mission.EndTime)
                {
                    newMissions.Add(mission);
                }
                else
                {
                    removedMissions.Add(mission);
                }
            }
            foreach (var stageKey in _campaignState.GetActive())
            {
                foreach (var node in _relevantStages[stageKey].MissionNodes)
                {
                    for (int i = 0; i < RollMissions(node.Frequency, node.Cap, _random); ++i)
                    {
                        var mission = _missionGenerator.Generate(stageKey, node, _time, _random.Next());
                        newMissions.Add(mission);
                        addedMissions.Add(mission);
                    }
                }
            }

            _missions = newMissions;
            foreach (var mission in removedMissions)
            {
                MissionRemoved?.Invoke(this, mission);
            }
            foreach (var mission in addedMissions)
            {
                MissionAdded?.Invoke(this, mission);
            }
        }

        private static int RollMissions(float frequency, int cap, Random random)
        {
            float p = 2f * frequency / cap;
            var r = random.NextSingle();
            if (r < p)
            {
                return (int)Math.Ceiling(cap * (p - r) / p);
            }
            return 0;
        }
    }
}
