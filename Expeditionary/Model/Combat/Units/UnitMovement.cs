namespace Expeditionary.Model.Combat.Units
{
    public class UnitMovement
    {
        public struct Hindrance
        {
            public UnitModifier Minimum { get; set; }
            public UnitModifier Maximum { get; set; }
            public UnitModifier Cap { get; set; }

            public float GetCost(int value)
            {
                if (value > Cap.GetValue())
                {
                    return float.PositiveInfinity;
                }
                return Minimum.GetValue() + (Maximum.GetValue() - Minimum.GetValue()) * (1f * value / Cap.GetValue());
            }
        }

        public Hindrance Roughness { get; set; }
        public Hindrance Softness { get; set; }
        public Hindrance WaterDepth { get; set; }

        public float GetCost(int roughness, int softness, int waterDepth)
        {
            return 1 + Roughness.GetCost(roughness) + Softness.GetCost(softness) + WaterDepth.GetCost(waterDepth);
        }
    }
}
