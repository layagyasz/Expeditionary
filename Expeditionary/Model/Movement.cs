namespace Expeditionary.Model
{
    public record class Movement
    {
        public record struct Hindrance
        {
            public int Restriction { get; set; }
            public int Roughness { get; set; }
            public int Slope { get; set; }
            public int Softness { get; set; }
            public int WaterDepth { get; set; }

            public Hindrance() { }

            public Hindrance(int restriction, int roughness, int slope, int softness, int waterDepth)
            {
                Roughness = roughness;
                Softness = softness;
                Restriction = restriction;
                Slope = slope;
                WaterDepth = waterDepth;
            }
        }

        public record struct CostFunction
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

        public CostFunction Restriction { get; set; } = new();
        public CostFunction Roughness { get; set; } = new();
        public CostFunction Slope { get; set; } = new();
        public CostFunction Softness { get; set; } = new();
        public CostFunction WaterDepth { get; set; } = new();

        public Movement() { }

        public Movement(
            CostFunction restriction,
            CostFunction roughness,
            CostFunction slope,
            CostFunction softness,
            CostFunction waterDepth)
        {
            Restriction = restriction;
            Roughness = roughness;
            Slope = slope;
            Softness = softness;
            WaterDepth = waterDepth;
        }

        public float GetCost(Hindrance hindrance)
        {
           return 1 + Restriction.GetCost(hindrance.Restriction) + Roughness.GetCost(hindrance.Roughness) 
                + Slope.GetCost(hindrance.Slope) + Softness.GetCost(hindrance.Softness)
                + WaterDepth.GetCost(hindrance.WaterDepth);
        }
    }
}
