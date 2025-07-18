using Expeditionary.Model.Mapping;
using OpenTK.Mathematics;

namespace Expeditionary.Model
{
    public record class Movement
    {
        public record struct Hindrance(int Restriction, int Roughness, int Slope, int Softness, int WaterDepth);

        public record struct CostFunction
        {
            public float Minimum { get; set; }
            public float Maximum { get; set; }
            public int Cap { get; set; } = 5;

            public CostFunction() { }

            public CostFunction(float minimum, float maximum, int cap)
            {
                Minimum = minimum;
                Maximum = maximum;
                Cap = cap;
            }

            public float GetCost(int value)
            {
                if (value > Cap)
                {
                    return float.PositiveInfinity;
                }
                return MathHelper.Lerp(Minimum, Maximum, 1f * value / Cap);
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

        public Hindrance GetMaxHindrance()
        {
            return new(Restriction.Cap, Roughness.Cap, Slope.Cap, Softness.Cap, WaterDepth.Cap);
        }

        public static Hindrance GetHindrance(Tile origin, Tile destination, Edge edge)
        {
            if (edge.Levels.ContainsKey(EdgeType.Road))
            {
                return new(Restriction: 0, Roughness: 0, Slope: 0, Softness: 0, WaterDepth: 0);
            }
            if (destination.Terrain.IsLiquid)
            {
                return new(Restriction: 0, Roughness: 0, Slope: 0, Softness: 0, WaterDepth: 5);
            }
            Hindrance h = destination.Hindrance;
            if (edge.Levels.ContainsKey(EdgeType.River))
            {
                h.WaterDepth = Math.Min(h.WaterDepth + 2, 5);
            }
            if (origin.Elevation != destination.Elevation)
            {
                h.Slope = Math.Abs(origin.Elevation - destination.Elevation);
            }
            h.Restriction = Math.Min(origin.Hindrance.Restriction, destination.Hindrance.Restriction);
            return h;
        }

        public static Hindrance Min(Hindrance left, Hindrance right)
        {
            return new(
                Math.Min(left.Restriction, right.Restriction),
                Math.Min(left.Roughness, right.Roughness), 
                Math.Min(left.Slope, right.Slope),
                Math.Min(left.Softness, right.Softness),
                Math.Min(left.WaterDepth, right.WaterDepth));
        }
    }
}
