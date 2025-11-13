using Expeditionary.Loader;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Mapping.Appearance;
using Expeditionary.Model.Mapping.Environments;
using Expeditionary.Model.Mapping.Generator;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Missions
{
    public record class MapSetup
    {
        public MapEnvironmentDefinition Environment { get; }
        public Vector2i Size { get; }
        public IEnumerable<CityGenerator.LayerParameters> ExtraLayers => _extraLayers;

        private readonly List<CityGenerator.LayerParameters> _extraLayers;

        public MapSetup(
            MapEnvironmentDefinition environment,
            Vector2i size, 
            IEnumerable<CityGenerator.LayerParameters> extraLayers)
        {
            Environment = environment;
            Size = size;
            _extraLayers = extraLayers.ToList();
        }

        public LoaderTaskNode<(Map, MapAppearance)> GenerateMap(LoaderStatus status, Random random)
        {
            var environment = Environment.GetEnvironment();
            environment.Parameters.Cities.Layers.InsertRange(0, _extraLayers);
            return MapGenerator.Generate(
                status, environment.Parameters, Size, seed: random.Next()).Map(map => (map, environment.Appearance));
        }
    }
}
