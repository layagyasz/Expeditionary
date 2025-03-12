using Cardamom.Collections;
using Expeditionary.Model.Factions;
using Expeditionary.Model.Missions.MissionTypes;

namespace Expeditionary.Model.Missions.MissionNodes
{
    public abstract record class MissionNodeBase : IMissionNode
    {
        public IEnvironmentProvider? Environment { get; set; }
        public EnumSet<MissionDifficulty> Difficulty { get; set; } = new();
        public EnumSet<MissionScale> Scale { get; set; } = new();
        public List<Faction> Attackers { get; set; } = new();
        public List<Faction> Defenders { get; set; } = new();

        public abstract Mission Create(MissionGenerationResources resources);
    }
}
