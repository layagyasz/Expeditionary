using OpenTK.Mathematics;

namespace Expeditionary.Model.Combat.Units
{
    public class Unit : IAsset
    {
        public int Id { get; }
        public Player Player { get; }
        public string TypeKey => Type.Key;
        public Vector3i Position { get; set; }
        public UnitType Type { get; }
        public float Movement { get; set; }

        public Unit(int id, Player player, UnitType type)
        {
            Id = id;
            Player = player;
            Type = type;

            Reset();
        }

        public void Reset()
        {
            Movement = Type.Speed;
        }
    }
}
