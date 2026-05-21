using Expeditionary.Model.Factions;

namespace Expeditionary.Model.Instances
{
    public class InstancePlayer
    {
        public int Id { get; }
        public Faction Faction { get; }
        public InstanceFormation Formation { get; }

        public InstancePlayer(int id, Faction faction, InstanceFormation formation)
        {
            Id = id;
            Faction = faction;
            Formation = formation;
        }
    }
}
