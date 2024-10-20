using OpenTK.Mathematics;

namespace Expeditionary.Model.Combat
{
    public interface IAsset
    {
        int Id { get; }
        string TypeKey { get; }
        public Vector3i Position { get; set; }
    }
}
