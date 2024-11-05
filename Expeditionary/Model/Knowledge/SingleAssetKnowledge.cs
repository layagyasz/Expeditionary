using OpenTK.Mathematics;

namespace Expeditionary.Model.Knowledge
{
    public record struct SingleAssetKnowledge(bool IsVisible, Vector3i? LastPosition);
}
