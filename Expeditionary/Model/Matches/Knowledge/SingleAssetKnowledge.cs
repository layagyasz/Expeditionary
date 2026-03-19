using OpenTK.Mathematics;

namespace Expeditionary.Model.Matches.Knowledge
{
    public record struct SingleAssetKnowledge(bool IsVisible, Vector3i? LastSeen);
}
