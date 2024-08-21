using OpenTK.Mathematics;

namespace Expeditionary.Coordinates
{
    public struct Offset2i
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Offset2i() { }

        public Offset2i(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return string.Format("Offset2i({0},{1})", X, Y);
        }

        public static Axial2i ToAxial(Offset2i x)
        {
            return new(x.X, x.Y - (x.X - (x.X & 1)) / 2);
        }

        public static Vector2 ToCartesian(Offset2i x)
        {
            return Axial2i.ToCartesian(ToAxial(x));
        }

        public static Hexagonal2i ToHexagonal(Offset2i x)
        {
            return Axial2i.ToHexagonal(ToAxial(x));
        }
    }
}
