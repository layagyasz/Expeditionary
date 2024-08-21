namespace Expeditionary.Coordinates
{
    public struct Barycentric2f
    {
        public float A { get; set; }
        public float B { get; set; }
        public float C { get; set; }

        public Barycentric2f() { }

        public Barycentric2f(float a, float b, float c)
        {
            A = a; 
            B = b;
            C = c;
        }

        public override string ToString()
        {
            return string.Format("Barycentric2f({0},{1},{2})", A, B, C);
        }

        public static float Sum(Barycentric2f x)
        {
            return x.A + x.B + x.C;
        }

        public static Barycentric2f Normalize(Barycentric2f x)
        {
            return x / Sum(x);
        }

        public static Barycentric2f operator *(Barycentric2f left, Barycentric2f right)
        {
            return new(left.A * right.A, left.B * right.B, left.C * right.C);
        }

        public static Barycentric2f operator /(Barycentric2f left, float right)
        {
            return new(left.A / right, left.B / right, left.C / right);
        }
    }
}
