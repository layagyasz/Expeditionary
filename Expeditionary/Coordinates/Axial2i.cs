using OpenTK.Mathematics;

namespace Expeditionary.Coordinates
{
    public struct Axial2i
    {
        public int Q { get; set; }
        public int R { get; set; }

        public Axial2i() { }

        public Axial2i(int q, int r)
        {
            Q = q;
            R = r;
        }

        private static readonly float s_Sqrt3 = MathF.Sqrt(3);
        public static Vector2 ToCartesian(Axial2i x)
        {
            return new(1.5f * x.Q, 0.5f * s_Sqrt3 * x.Q + s_Sqrt3 * x.R);
        }

        public static Hexagonal2i ToHexagonal(Axial2i x)
        {
            return new(x.Q, x.R, -x.Q - x.R);
        }

        public static Offset2i ToOffset(Axial2i x)
        {
            return new(x.Q, x.R + (x.Q - (x.Q & 1)) / 2);
        }

        public static Axial2i operator +(Axial2i left, Axial2i right)
        {
            return new(left.Q + right.Q, left.R + right.R);
        }

        public static Axial2i operator -(Axial2i left, Axial2i right)
        {
            return new(left.Q - right.Q, left.R - right.R);
        }
    }
}
