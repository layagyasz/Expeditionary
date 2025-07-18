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
            var map = Map.Create(size, parameters.Terrain.ElevationLevels);
            TerrainGenerator.Generate(parameters.Terrain, map, random);
            var cores = CityGenerator.Generate(parameters.Cities, map, random);
            TransportGenerator.Generate(parameters.Transport, cores, map, random);
            Hindrance(map);
            return map;
        }

        private static void Hindrance(Map map)
        {
            for (int i = 0; i < map.Width; ++i)
            {
                for (int j = 0; j < map.Height; ++j)
                {
                    var tile = map.Get(i, j)!;
                    var hindrance = new Movement.Hindrance();
                    if (tile.Terrain.Foliage != null)
                    {
                        hindrance.Roughness = 2;
                        hindrance.Restriction = 3;
                    }
                    else if (tile.Terrain.Soil != null)
                    {
                        hindrance.Roughness = 1;
                    }
                    else
                    {
                        hindrance.Roughness = 3;
                    }
                    hindrance.Softness = (int)(3 * tile.Moisture);
                    if (tile.Terrain.HasGroundCover)
                    {
                        hindrance.Roughness = Math.Max(0, hindrance.Roughness - 1);
                        hindrance.Softness = 3;
                    }
                    tile.Hindrance = hindrance;
                }
            }
        }
    }
}
