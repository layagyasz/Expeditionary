using Expeditionary.Model.Missions;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Units
{
    public class Unit : IAsset
    {
        public int Id { get; }
        public Player Player { get; }
        public string TypeKey => Type.Key;
        public Vector3i Position { get; set; }
        public UnitType Type { get; }
        public int Number { get; private set; }
        public float Movement { get; set; }
        public bool Attacked { get; set; }
        public bool Destroyed { get; private set; }

        public UnitQuantity UnitQuantity => new(1, Type.Points);


        public Unit(int id, Player player, UnitType type)
        {
            Id = id;
            Player = player;
            Type = type;
            Number = (int)type.Intrinsics.Number.GetValue();

            Reset();
        }

        public void Damage(int kills)
        {
            Number -= kills;
        }

        public void Destroy()
        {
            Destroyed = true;
            Position = default;
        }

        public void Reset()
        {
            Movement = Type.Speed;
            Attacked = false;
        }
    }
}
