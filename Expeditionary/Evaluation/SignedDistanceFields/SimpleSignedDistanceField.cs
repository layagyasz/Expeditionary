using Expeditionary.Hexagons;
using OpenTK.Mathematics;

namespace Expeditionary.Evaluation.SignedDistanceFields
{
    public class SimpleSignedDistanceField : ISignedDistanceField
    {
        public Vector3i Hex { get; }
        public int MaxInternalDistance { get; }
        public int MaxExternalDistance { get; }

        public SimpleSignedDistanceField(Vector3i hex, int distance, int maxDistance)
        {
            Hex = hex;
            MaxInternalDistance = distance;
            MaxExternalDistance = maxDistance - distance;
        }

        public int Get(Vector3i hex)
        {
            return Geometry.GetCubicDistance(Hex, hex) - MaxInternalDistance;
        }
    }
}
