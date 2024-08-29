using OpenTK.Mathematics;

namespace Expeditionary.Model.Mapping
{
    public class Terrain
    {
        public bool IsLiquid { get; set; }
        public Vector3 Stone { get; set; }
        public Vector3? Soil { get; set; }
        public Vector2? Brush { get; set; }
        public Vector2? Foliage { get; set; }
    }
}
