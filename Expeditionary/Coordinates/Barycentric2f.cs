namespace Expeditionary.Coordinates
{
    public struct Barycentric2f
    {
        public float U { get; set; }
        public float V { get; set; }
        public float W { get; set; }

        public Barycentric2f() { }

        public Barycentric2f(float u, float v, float w)
        {
            U = u; 
            V = v;
            W = w;
        }

        public override string ToString()
        {
            return string.Format("Barycentric2f({0},{1},{2})", U, V, W);
        }

        public static float Distance(Barycentric2f left, Barycentric2f right)
        {
            var diff = left - right;
            return Math.Max(diff.U, Math.Max(diff.V, diff.W));
        }

        public static float Sum(Barycentric2f x)
        {
            return x.U + x.V + x.W;
        }

        public static Barycentric2f Normalize(Barycentric2f x)
        {
            return x / Sum(x);
        }

        public static Barycentric2f operator -(Barycentric2f left, Barycentric2f right)
        {
            return new(left.U - right.U, left.V - right.V, left.W - right.W);
        }

        public static Barycentric2f operator *(Barycentric2f left, Barycentric2f right)
        {
            return new(left.U * right.U, left.V * right.V, left.W * right.W);
        }

        public static Barycentric2f operator /(Barycentric2f left, float right)
        {
            return new(left.U / right, left.V / right, left.W / right);
        }
    }
}
