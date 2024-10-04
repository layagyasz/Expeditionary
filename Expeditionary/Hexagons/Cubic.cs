using Cardamom.Mathematics.Coordinates.Projections;
using OpenTK.Mathematics;

namespace Expeditionary.Hexagons
{
    public static class Cubic
    {
        public class Cartesian : IProjection<Vector3, Vector2>
        {
            public static readonly Cartesian Instance = new();

            public Vector2 Project(Vector3 hex)
            {
                return Axial.Cartesian.Instance.Project(hex.Xy);
            }

            public Vector3 Wrap(Vector2 cartesian)
            {
                var axial = Axial.Cartesian.Instance.Wrap(cartesian);
                return new(axial.X, axial.Y, -axial.X - axial.Y);
            }
        }

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

        public class TriangularOffset : IProjection<Vector3i, Vector2i>
        {
            public static readonly TriangularOffset Instance = new();

            public Vector2i Project(Vector3i hex)
            {
                int dir = (1 - hex.X - hex.Y - hex.Z) >> 1;
                int x = hex.X + dir;
                return new(x, 2 * hex.Y + x + dir);
            }

            public Vector3i Wrap(Vector2i offset)
            {
                int dir = 1 & (offset.X ^ offset.Y);
                int q = offset.X - dir;
                int r = (offset.Y - offset.X - dir) >> 1;
                return new(q, r, 1 - q - r - 2 * dir);
            }
        }
    }
}
