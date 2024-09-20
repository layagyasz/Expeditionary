using OpenTK.Mathematics;

namespace Expeditionary.Model.Mapping.Generator
{
    public class TerrainParameters
    {
        public float LiquidLevel { get; set; } = 0.25f;
        public Vector3 Stone { get; set; } = new(1, 1, 1);
        public float SoilCover { get; set; } = 0.9f;
        public Vector3 Soil { get; set; } = new(1, 1, 1);
        public float BrushCover { get; set; } = 0.9f;
        public float FoliageCover { get; set; } = 0.6f;
        public float LiquidBonus = 0.2f;
        public int Rivers { get; set; }
    }
}
