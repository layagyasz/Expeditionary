using Expeditionary.View.Common.Gradients;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Mapping.Appearance
{
    public record class PlantColoringSet
    {
        public IColoring HotDry { get; set; } = new IColoring.StaticColoring(Color4.Magenta);
        public IColoring HotWet { get; set; } = new IColoring.StaticColoring(Color4.Magenta);
        public IColoring ColdDry { get; set; } = new IColoring.StaticColoring(Color4.Magenta);
        public IColoring ColdWet { get; set; } = new IColoring.StaticColoring(Color4.Magenta);

        public Gradient2 Materialize(Color4 solarOutput)
        {
            return new()
            {
                TopRight = HotDry.Get(solarOutput),
                BottomRight = HotWet.Get(solarOutput),
                TopLeft = ColdDry.Get(solarOutput),
                BottomLeft = ColdWet.Get(solarOutput)
            };
        }
    }
}
