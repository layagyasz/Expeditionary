using Cardamom.Utils.Generators.Samplers;

namespace Expeditionary.Model.Mapping.Generator
{
    public class CityParameters
    {
        public int Cores { get; set; }
        public int Candidates { get; set; }
        public ISampler Size { get; set; } = new NormalSampler(10, 5);
        public float BasePenalty { get; set; } = 0.1f;
        public float SlopePenalty { get; set; } = 1;
        public float ElevationPenalty { get; set; } = 1;
        public float NoLiquidPenalty { get; set; } = 2;
        public float NoRiverPenalty { get; set; } = 1;
    }
}
