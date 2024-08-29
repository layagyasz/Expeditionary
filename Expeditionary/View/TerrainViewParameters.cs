using OpenTK.Mathematics;

namespace Expeditionary.View
{
    public class TerrainViewParameters
    {
        public Color4 Liquid { get; set; }
        public GradientBarycentric Stone { get; set; }
        public GradientBarycentric Soil { get; set; }
        public Gradient2 Brush { get; set; }
        public Gradient2 Foliage { get; set; }
    }
}
