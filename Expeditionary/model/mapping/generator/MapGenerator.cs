using OpenTK.Mathematics;

namespace Expeditionary.Model.Mapping.Generator
{
    public class MapGenerator
    {
        public class Parameters
        {
            public TerrainGenerator.Parameters Terrain { get; set; } = new();
            public CityGenerator.Parameters Cities { get; set; } = new();
        }

        private TerrainGenerator _terrainGenerator = new();

        public Map Generate(Parameters parameters, Vector2i size, int seed)
        {
            var random = new Random(seed);
            var map = new Map(size);
            _terrainGenerator.Generate(parameters.Terrain, map, random);
            CityGenerator.Generate(parameters.Cities, map, random);
            return map;
        }
    }
}
