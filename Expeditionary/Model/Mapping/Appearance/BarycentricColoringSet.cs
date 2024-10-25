using Expeditionary.View.Common.Gradients;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Mapping.Appearance
{
    public record class BarycentricColoringSet
    {
        public IColoring A { get; set; } = new IColoring.StaticColoring(Color4.Magenta);
        public IColoring B { get; set; } = new IColoring.StaticColoring(Color4.Magenta);
        public IColoring C { get; set; } = new IColoring.StaticColoring(Color4.Magenta);

        public GradientBarycentric Materialize(Color4 solarOutput)
        {
            return new GradientBarycentric()
            {
                A = A.Get(solarOutput),
                B = B.Get(solarOutput),
                C = C.Get(solarOutput)
            };
        }
    }
}
