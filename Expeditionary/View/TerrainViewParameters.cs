using OpenTK.Mathematics;

namespace Expeditionary.View
{
    public class TerrainViewParameters
    {
        public record class Stone(Color4[] Colors) { }

        public Stone? StoneParameters { get; set; }
    }
}
