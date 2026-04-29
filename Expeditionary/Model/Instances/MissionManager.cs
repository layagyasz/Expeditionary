using Expeditionary.Model.Campaigns;
using Expeditionary.Model.Factions;
using Expeditionary.Model.Missions;
using Expeditionary.Model.Missions.Generator;

namespace Expeditionary.Model.Instances
{
    public class MissionManager
    {
        public EventHandler<Mission>? MissionAdded { get; set; }
        public EventHandler<Mission>? MissionRemoved { get; set; }

        public IEnumerable<Mission> Missions => _missions;

        private readonly HashSet<Faction> _factions;
        private readonly MissionGenerator _missionGenerator;
        private readonly List<Campaign> _campaigns;
        private readonly Random _random;

        private long _time;
        private List<Mission> _missions;

        public MissionManager(
            IEnumerable<Faction> factions, 
            MissionGenerator missionGenerator,
            IEnumerable<Campaign> campaigns,
            Random random)
        {
            _factions = factions.ToHashSet();
            _missionGenerator = missionGenerator;
            _campaigns = campaigns.ToList();
            _random = random;
            _missions = new();
        }

        public void Step()
        {
            _time++;
            var newMissions = new List<Mission>();
            var removedMissions = new List<Mission>();
            var addedMissions = new List<Mission>();
            foreach (var mission in _missions)
            {
                if (_time < mission.StartTime + mission.Duration)
                {
                    newMissions.Add(mission);
                }
                else
                {
                    removedMissions.Add(mission);
                }
            }
            foreach (var campaign in _campaigns.Where(campaign => _factions.Intersect(campaign.Factions).Any()))
            {
                // TODO: Take campaign state into account
                foreach (var node in campaign.Nodes.Find(node => node.Id == campaign.InitialNodeId)!.MissionNodes)
                {
                    for (int i = 0; i < RollMissions(node.Frequency, node.Cap, _random); ++i)
                    {
                        var mission = _missionGenerator.Generate(node, _time, _random.Next());
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
