using OpenTK.Mathematics;

namespace Expeditionary.Model.Units
{
    public interface IAsset
    {
        int Id { get; }
        string Name { get; }
        string TypeKey { get; }
        public Vector3i? Position { get; set; }
        public bool IsDestroyed { get; }
        public bool IsPassenger { get; set; }
        public AssetValue Value { get; }
        public void Reset();

        public string? ToString()
        {
            return $"[IAsset: Id={Id}, TypeKey={TypeKey}]";
        }
    }
}
