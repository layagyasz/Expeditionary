using OpenTK.Mathematics;

namespace Expeditionary.Model.Knowledge
{
    public record class SingleAssetKnowledge
    {
        public bool IsVisible { get; set; }
        public Vector3i? LastSeen { get; set; }
    }
}
