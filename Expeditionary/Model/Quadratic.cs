namespace Expeditionary.Model
{
    public struct Quadratic
    {
        public float A { get; set; }
        public float B { get; set; }
        public float C { get; set; }

        public Quadratic(float a, float b, float c)
        {
            A = a;
            B = b;
            C = c;
        }

        public float Evaluate(float x)
        {
            return A * x * x + B * x + C;
        }
    }
}
