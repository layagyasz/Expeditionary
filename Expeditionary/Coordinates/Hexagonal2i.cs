using OpenTK.Mathematics;

namespace Expeditionary.Coordinates
{
    public struct Hexagonal2i
    {
        public int Q { get; set; }
        public int R { get; set; }
        public int S { get; set; }

        public Hexagonal2i() { }

        public Hexagonal2i(int q, int r, int s)
        {
            Q = q;
            R = r;
            S = s;
        }

        public static Axial2i ToAxial(Hexagonal2i x)
        {
            return new(x.Q, x.R);
        }

        public static Vector2 ToCartesian(Hexagonal2i x)
        {
            return Axial2i.ToCartesian(ToAxial(x));
        }

        public static Offset2i ToOffset(Hexagonal2i x)
        {
            return Axial2i.ToOffset(ToAxial(x));
        }
    }
}
