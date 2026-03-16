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
        public Vector3i Position { get; set; }
        public AssetStatus Status { get; set; } = AssetStatus.Reserved;
        public bool IsActive => Status == AssetStatus.Active;
        public bool IsDestroyed => Status == AssetStatus.Destroyed;
        public bool IsReserved => Status == AssetStatus.Reserved;
        public bool IsPassenger { get; set; }
        public IEnumerable<UnitTag> Tags => Type.Tags;
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
