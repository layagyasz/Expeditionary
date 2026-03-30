using Expeditionary.Model.Units;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Matches.Assets
{
    public interface IMatchAsset
    {
        int Id { get; }
        string Name { get; }
        string TypeKey { get; }
        Vector3i Position { get; set; }
        MatchAssetStatus Status { get; set; }
        bool IsActive => Status == MatchAssetStatus.Active;
        bool IsDestroyed => Status == MatchAssetStatus.Destroyed;
        bool IsReserved => Status == MatchAssetStatus.Reserved;
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
