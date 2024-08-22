using Expeditionary.Coordinates;

namespace Expeditionary.Model.Mapping
{
    public class TerrainParameters
    {
        public float LiquidLevel { get; set; } = 0.25f;
        public Barycentric2f Stone { get; set; } = new(1, 1, 1);
        public float SoilCover { get; set; } = 1f;
        public Barycentric2f Soil { get; set; } = new(1, 1, 1);
    }
}
