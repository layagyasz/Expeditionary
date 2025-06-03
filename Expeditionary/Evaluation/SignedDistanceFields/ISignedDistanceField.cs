using OpenTK.Mathematics;

namespace Expeditionary.Evaluation.SignedDistanceFields
{
    public interface ISignedDistanceField
    {
        int MaxInternalDistance { get; }
        int MaxExternalDistance { get; }
        int Get(Vector3i hex);
    }
}
