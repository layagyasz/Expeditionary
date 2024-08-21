using Expeditionary.Coordinates;

namespace Expeditionary.Model.Mapping
{
    public  class TerrainParameters
    {
        public record struct Stone(Barycentric2f Weight) { } 

        public Stone StoneParameters { get; set; } = new Stone(new(1, 1, 1));
    }
}
