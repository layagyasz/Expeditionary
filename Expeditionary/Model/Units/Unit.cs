using Expeditionary.Model.Missions;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Units
{
    public class Unit : IAsset
    {
        public int Id { get; }
        public Player Player { get; }
        public string TypeKey => Type.Key;
        public Vector3i? Position { get; set; }
        public bool IsDestroyed { get; private set; }
        public UnitType Type { get; }
        public int Number { get; private set; }
        public float Movement { get; set; }
        public bool Attacked { get; set; }

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
            IsDestroyed = true;
            Position = default;
        }

        public bool IsActive()
        {
            return !IsDestroyed && Position != null;
        }

        public void Reset()
        {
            Movement = Type.Speed;
            Attacked = false;
        }

        public override string ToString()
        {
            return $"[Unit: Id={Id}, TypeKey={TypeKey}]";
        }
    }
}
