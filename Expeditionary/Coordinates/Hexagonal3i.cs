using OpenTK.Mathematics;

namespace Expeditionary.Coordinates
{
    public struct Hexagonal3i
    {
        public int Q { get; set; }
        public int R { get; set; }
        public int S { get; set; }

        public Hexagonal3i() { }

        public Hexagonal3i(int q, int r, int s)
        {
            Q = q;
            R = r;
            S = s;
        }

        public static Axial2i ToAxial(Hexagonal3i x)
        {
            return new(x.Q, x.R);
        }

        public static Vector2 ToCartesian(Hexagonal3i x)
        {
            return Axial2i.ToCartesian(ToAxial(x));
        }

        public static Offset2i ToOffset(Hexagonal3i x)
        {
            return Axial2i.ToOffset(ToAxial(x));
        }
    }
}
