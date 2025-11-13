using Expeditionary.Loader;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Mapping.Generator
{
    public class MapGenerator
    {
        private static readonly object o_Elevation = new();
        private static readonly object o_Terrain = new();
        private static readonly object o_Habitation = new();
        private static readonly object o_Hindrance = new();
        private static readonly object o_Transport = new();

        public class Parameters
        {
            public TerrainGenerator.Parameters Terrain { get; set; } = new();
            public CityGenerator.Parameters Cities { get; set; } = new();
            public List<TransportGenerator.Parameters> Transport { get; set; } = new();
        }

        public static LoaderTaskNode<Map> Generate(LoaderStatus status, Parameters parameters, Vector2i size, int seed)
        {
            status.AddSegments(o_Elevation, o_Terrain, o_Habitation, o_Hindrance, o_Transport);
            status.AddWork(o_Elevation, 1);
            status.AddWork(o_Terrain, 1);
            status.AddWork(o_Habitation, 1);
            status.AddWork(o_Hindrance, 1);
            status.AddWork(o_Transport, 1);

            return new SourceLoaderTask<Map>(() => GenerateAux(status, parameters, size, seed), isGL: true);

        }

        private static Map GenerateAux(LoaderStatus status, Parameters parameters, Vector2i size, int seed)
        {
            var random = new Random(seed);
            var map = Map.Create(size, parameters.Terrain.ElevationLevels);
            status.DoWork(o_Elevation);

            TerrainGenerator.Generate(parameters.Terrain, map, random);
            status.DoWork(o_Terrain);

            var cores = CityGenerator.Generate(parameters.Cities, map, random);
            status.DoWork(o_Habitation);

            Hindrance(map);
            status.DoWork(o_Hindrance);

            TransportGenerator.Generate(parameters.Transport, cores, map, random);
            status.DoWork(o_Transport);

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
