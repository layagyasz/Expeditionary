using Expeditionary.Model.Factions;

namespace Expeditionary.Model.Instances
{
    public class InstancePlayer
    {
        public int Id { get; }
        public Faction Faction { get; }
        public IEnumerable<InstanceFormation> Formations => _formations;

        private readonly List<InstanceFormation> _formations = new();

        public InstancePlayer(int id, Faction faction)
        {
            Id = id;
            Faction = faction;
        }

        public void AddFormation(InstanceFormation formation)
        {
            _formations.Add(formation);
        }
    }
}
