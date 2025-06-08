using OpenTK.Mathematics;

namespace Expeditionary.Model.Mapping.Appearance
{
    public interface IColoring
    {
        Color4 Get(Color4 solarOutput);

        public record class SolarOutputOffsetColoring(Color4 Offset) : IColoring
        {
            public Color4 Get(Color4 solarOutput)
            {
                return Color4.FromHsv(Colors.CombineHsv((Vector4)solarOutput, (Vector4)Offset));
            }
        }

        public record class StaticColoring(Color4 Color) : IColoring
        {
            public Color4 Get(Color4 solarOutput)
            {
                return Color;
            }
        }
    }
}
