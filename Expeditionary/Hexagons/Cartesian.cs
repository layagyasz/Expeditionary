using OpenTK.Mathematics;

namespace Expeditionary.Hexagons
{
    public static class Cartesian
    {
        private static readonly float s_Sqrt3 = MathF.Sqrt(3);

        public static Vector2 FromAxial(Vector2i axial)
        {
            return new(1.5f * axial.X, 0.5f * s_Sqrt3 * axial.X + s_Sqrt3 * axial.Y);
        }

        public static Vector2 FromOffset(Vector2i offset)
        {
            return FromAxial(Axial.Offset.Instance.Wrap(offset));
        }
    }
}
