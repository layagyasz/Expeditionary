namespace Expeditionary.Model
{
    public class Movement
    {
        public struct Hindrance
        {
            public int Roughness { get; set; }
            public int Softness { get; set; }
            public int WaterDepth { get; set; }

            public Hindrance() { }

            public Hindrance(int roughness, int softness, int waterDepth)
            {
                Roughness = roughness;
                Softness = softness;
                WaterDepth = waterDepth;
            }
        }

        public struct CostFunction
        {
            public Modifier Minimum { get; set; } = Modifier.None;
            public Modifier Maximum { get; set; } = Modifier.None;
            public Modifier Cap { get; set; } = new(1, 5);

            public CostFunction() { }

            public CostFunction(Modifier minimum, Modifier maximum, Modifier cap)
            {
                Minimum = minimum;
                Maximum = maximum;
                Cap = cap;
            }

            public float GetCost(int value)
            {
                if (value > Cap.GetValue())
                {
                    return float.PositiveInfinity;
                }
                return Minimum.GetValue() + (Maximum.GetValue() - Minimum.GetValue()) * (1f * value / Cap.GetValue());
            }
        }

        public CostFunction Roughness { get; set; } = new();
        public CostFunction Softness { get; set; } = new();
        public CostFunction WaterDepth { get; set; } = new();

        public Movement() { }

        public Movement(CostFunction roughness, CostFunction softness, CostFunction waterDepth)
        {
            Roughness = roughness;
            Softness = softness;
            WaterDepth = waterDepth;
        }

        public float GetCost(Hindrance hindrance)
        {
            return 1 + Roughness.GetCost(hindrance.Roughness) + Softness.GetCost(hindrance.Softness) 
                + WaterDepth.GetCost(hindrance.WaterDepth);
        }
    }
}
