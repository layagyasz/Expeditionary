using OpenTK.Mathematics;

namespace Expeditionary.Model.Units
{
    public class Unit : IAsset
    {
        public int Id { get; }
        public string Name => $"{Type.Name}(#{Id})";
        public Player Player { get; }
        public UnitType Type { get; }
        public string TypeKey => Type.Key;
        public Vector3i? Position { get; set; }
        public bool IsDestroyed { get; private set; }
        public bool IsPassenger { get; set; }
        public int Number { get; private set; }
        public int Actions { get; private set; }
        public IAsset? Passenger { get; set; }
        public AssetValue Value => new(1, Type.Points);

        public Unit(int id, Player player, UnitType type)
        {
            Id = id;
            Player = player;
            Type = type;
            Number = (int)type.Intrinsics.Number.GetValue();

            Reset();
        }

        public void ConsumeAction()
        {
            Actions -= 1;
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
            Actions = 1;
        }

        public override string ToString()
        {
            return $"[Unit: Id={Id}, TypeKey={TypeKey}]";
        }
    }
}
