using OpenTK.Mathematics;

namespace Expeditionary.Model.Mapping.Appearance
{
    public interface IColoring
    {
        Color4 Get(Color4 solarOutput);

        public record class SolarOutputOffsetColoring(Color4 Offset) : IColoring
        {
            public Color4 Get(Color4 solarOutput)
            {
                return Color4.FromHsv(CombineHsv((Vector4)solarOutput, (Vector4)Offset));
            }

            private static Vector4 CombineHsv(Vector4 color, Vector4 adjustment)
            {
                var hsv = color;
                hsv.X = (hsv.X + 1 + adjustment.X) % 1;
                hsv.Y = MathHelper.Clamp(hsv.Y * adjustment.Y, 0, 1);
                hsv.Z = MathHelper.Clamp(hsv.Z * adjustment.Z, 0, 1);
                return hsv;
            }
        }

        public record class StaticColoring(Color4 Color) : IColoring
        {
            public Color4 Get(Color4 solarOutput)
            {
                return Color;
            }
        }
    }
}
