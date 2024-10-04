using Cardamom.Mathematics.Coordinates.Projections;
using OpenTK.Mathematics;

namespace Expeditionary.Hexagons
{
    public static class Axial
    {
        public class Cartesian : IProjection<Vector2, Vector2>
        {
            private static readonly float s_Sqrt3 = MathF.Sqrt(3);

            public static readonly Cartesian Instance = new();

            public Vector2 Project(Vector2 axial)
            {
                return new(1.5f * axial.X, 0.5f * s_Sqrt3 * axial.X + s_Sqrt3 * axial.Y);
            }

            public Vector2 Wrap(Vector2 axial)
            {
                return new(0.666666666667f * axial.X, 0.333333333333f * (s_Sqrt3 * axial.Y - axial.X));
            }
        }

        public class Offset : IProjection<Vector2i, Vector2i>
        {
            public static readonly Offset Instance = new();

            public Vector2i Project(Vector2i axial)
            {
                int dir = axial.X & 1;
                return new(axial.X, axial.Y + ((axial.X - dir) >> 1));
            }

            public Vector2i Wrap(Vector2i offset)
            {
                int dir = offset.X & 1;
                return new(offset.X, offset.Y - ((offset.X - dir) >> 1));
            }
        }
    }
}
