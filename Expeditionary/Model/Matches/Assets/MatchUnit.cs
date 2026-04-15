using Expeditionary.Model.Units;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Matches.Assets
{
    public class MatchUnit : IMatchAsset
    {
        public int Id { get; }
        public int InstanceId { get; }
        public string Name => $"{Type.Name}(#{Id})";
        public Player Player { get; }
        public UnitType Type { get; }
        public string TypeKey => Type.Key;
        public Vector3i Position { get; set; }
        public MatchAssetStatus Status { get; set; } = MatchAssetStatus.Reserved;
        public bool IsActive => Status == MatchAssetStatus.Active;
        public bool IsDestroyed => Status == MatchAssetStatus.Destroyed;
        public bool IsReserved => Status == MatchAssetStatus.Reserved;
        public bool IsPassenger { get; set; }
        public IEnumerable<UnitTag> Tags => Type.Tags;
        public int Number { get; private set; }
        public int Actions { get; private set; }
        public IMatchAsset? Passenger { get; set; }
        public AssetValue Value => new(1, Type.Points);

        public MatchUnit(int id, int instanceId, Player player, UnitType type)
        {
            Id = id;
            InstanceId = instanceId;
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
