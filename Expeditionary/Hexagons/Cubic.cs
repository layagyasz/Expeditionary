using Cardamom.Mathematics.Coordinates.Projections;
using OpenTK.Mathematics;

namespace Expeditionary.Hexagons
{
    public static class Cubic
    {
        public class HexagonalOffset : IProjection<Vector3i, Vector2i>
        {
            public static readonly HexagonalOffset Instance = new();

            public Vector2i Project(Vector3i hex)
            {
                return Axial.Offset.Instance.Project(hex.Xy);
            }

            public Vector3i Wrap(Vector2i offset)
            {
                var axial = Axial.Offset.Instance.Wrap(offset);
                return new(axial, -axial.X - axial.Y);
            }
        }
    }
}
