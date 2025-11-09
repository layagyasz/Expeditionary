using Expeditionary.Model.Missions.Generator;

namespace Expeditionary.Model.Missions
{
    public class MissionManager
    {
        public EventHandler<Mission>? MissionAdded { get; set; }
        public EventHandler<Mission>? MissionRemoved { get; set; }

        public IEnumerable<Mission> Missions => _missions;

        private readonly MissionGenerator _missionGenerator;
        private readonly List<MissionNode> _nodes;
        private readonly Random _random;

        private long _time;
        private List<Mission> _missions;

        public MissionManager(MissionGenerator missionGenerator, IEnumerable<MissionNode> nodes, Random random)
        {
            _missionGenerator = missionGenerator;
            _nodes = nodes.ToList();
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
            foreach (var node in _nodes)
            {
                for (int i=0; i<RollMissions(node.Frequency, node.Cap, _random); ++i)
                {
                    var mission = _missionGenerator.Generate(node, _time, _random.Next());
                    newMissions.Add(mission);
                    addedMissions.Add(mission);
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
            var r = random.NextSingle();
            if (r < frequency)
            {
                var v = (int)(cap * (1f - MathF.Sqrt(1f - r / frequency))) + 1;
                return v;
            }
            return 0;
        }
    }
}
