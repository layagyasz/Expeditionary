using OpenTK.Mathematics;

namespace Expeditionary.Model.Units
{
    public interface IAsset
    {
        int Id { get; }
        string TypeKey { get; }
        public Vector3i Position { get; set; }
        public void Reset();

        public string? ToString()
        {
            return $"[Asset: Id={Id}, TypeKey={TypeKey}]";
        }
    }
}
