using OpenTK.Mathematics;

namespace Expeditionary.Model.Mapping
{
    public class MapGenerator
    {
        public Map Generate(Vector2i size, int seed)
        {
            // TODO: Implement generation.
            var builder = new Map.Builder(size);
            return builder.Build();
        }
    }
}
