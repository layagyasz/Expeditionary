using OpenTK.Mathematics;

namespace Expeditionary.View.Common.Gradients
{
    public struct Gradient2
    {
        public Color4 TopLeft { get; set; }
        public Color4 BottomLeft { get; set; }
        public Color4 TopRight { get; set; }
        public Color4 BottomRight { get; set; }

        public Color4 Interpolate(Vector2 v)
        {
            return Lerp(Lerp(TopLeft, TopRight, v.X), Lerp(BottomLeft, BottomRight, v.X), v.Y);
        }

        private static Color4 Lerp(Color4 a, Color4 b, float v)
        {
            return (Color4)((1 - v) * (Vector4)a + v * (Vector4)b);
        }
    }
}
