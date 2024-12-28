using Expeditionary.View.Common.Gradients;
using OpenTK.Mathematics;

namespace Expeditionary.View.Mapping
{
    public record class TerrainViewParameters
    {
        public Color4 Liquid { get; set; }
        public Color4 GroundCover { get; set; }
        public GradientBarycentric Stone { get; set; }
        public GradientBarycentric Soil { get; set; }
        public Gradient2 Brush { get; set; }
        public Gradient2 Foliage { get; set; }
    }
}
