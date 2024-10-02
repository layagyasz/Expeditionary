using Expeditionary.Model.Factions;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Combat.Units
{
    public class Unit : IAsset
    {
        public int Id { get; }
        public string TypeKey => Type.Key;
        public Faction Faction { get; }
        public Vector3i Position { get; set; }
        public UnitType Type { get; }

        public Unit(int id, Faction faction, UnitType type)
        {
            Id = id;
            Faction = faction;
            Type = type;
        }
    }
}
