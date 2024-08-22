using Expeditionary.Coordinates;

namespace Expeditionary.Model.Mapping
{
    public class Terrain
    {
        public int Stone { get; set; }
        public Barycentric2f? Soil { get; set; }
    }
}
