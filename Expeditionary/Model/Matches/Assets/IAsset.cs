using Expeditionary.Model.Units;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Matches.Assets
{
    public interface IAsset
    {
        int Id { get; }
        string Name { get; }
        string TypeKey { get; }
        Vector3i Position { get; set; }
        AssetStatus Status { get; set; }
        bool IsActive => Status == AssetStatus.Active;
        bool IsDestroyed => Status == AssetStatus.Destroyed;
        bool IsReserved => Status == AssetStatus.Reserved;
        bool IsPassenger { get; set; }
        IEnumerable<UnitTag> Tags { get; }
        AssetValue Value { get; }
        void Reset();

        public string? ToString()
        {
            return $"[IAsset: Id={Id}, TypeKey={TypeKey}]";
        }
    }
}
