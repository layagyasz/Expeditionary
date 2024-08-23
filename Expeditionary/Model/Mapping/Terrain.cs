using OpenTK.Mathematics;

namespace Expeditionary.Model.Mapping
{
    public class Terrain
    {
        public bool IsLiquid { get; set; }
        public int Stone { get; set; }
        public Vector3? Soil { get; set; }
    }
}
