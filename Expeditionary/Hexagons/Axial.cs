using Cardamom.Mathematics.Coordinates.Projections;
using OpenTK.Mathematics;

namespace Expeditionary.Hexagons
{
    public static class Axial
    {
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
