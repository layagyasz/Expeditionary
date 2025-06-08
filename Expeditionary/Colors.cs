using OpenTK.Mathematics;

namespace Expeditionary
{
    public static class Colors
    {
        public static Vector4 CombineHsv(Vector4 color, Vector4 adjustment)
        {
            var hsv = color;
            hsv.X = (hsv.X + 1 + adjustment.X) % 1;
            hsv.Y = MathHelper.Clamp(hsv.Y * adjustment.Y, 0, 1);
            hsv.Z = MathHelper.Clamp(hsv.Z * adjustment.Z, 0, 1);
            return hsv;
        }
    }
}
