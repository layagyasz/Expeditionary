namespace Expeditionary.Model
{
    public class Movement
    {
        public struct Hindrance
        {
            public Modifier Minimum { get; set; } = Modifier.None;
            public Modifier Maximum { get; set; } = Modifier.None;
            public Modifier Cap { get; set; } = new(1, 5);

            public Hindrance() { }

            public float GetCost(int value)
            {
                if (value > Cap.GetValue())
                {
                    return float.PositiveInfinity;
                }
                return Minimum.GetValue() + (Maximum.GetValue() - Minimum.GetValue()) * (1f * value / Cap.GetValue());
            }
        }

        public Hindrance Roughness { get; set; } = new();
        public Hindrance Softness { get; set; } = new();
        public Hindrance WaterDepth { get; set; } = new();

        public float GetCost(int roughness, int softness, int waterDepth)
        {
            return 1 + Roughness.GetCost(roughness) + Softness.GetCost(softness) + WaterDepth.GetCost(waterDepth);
        }
    }
}
