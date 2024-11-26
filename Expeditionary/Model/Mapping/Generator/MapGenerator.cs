using OpenTK.Mathematics;

namespace Expeditionary.Model.Mapping.Generator
{
    public class MapGenerator
    {
        public class Parameters
        {
            public TerrainGenerator.Parameters Terrain { get; set; } = new();
            public CityGenerator.Parameters Cities { get; set; } = new();
            public List<TransportGenerator.Parameters> Transport { get; set; } = new();
        }

        public static Map Generate(Parameters parameters, Vector2i size, int seed)
        {
            var random = new Random(seed);
            var map = new Map(size, parameters.Terrain.ElevationLevels);
            TerrainGenerator.Generate(parameters.Terrain, map, random);
            var cores = CityGenerator.Generate(parameters.Cities, map, random);
            TransportGenerator.Generate(parameters.Transport, cores, map, random);
            return map;
        }
    }
}
