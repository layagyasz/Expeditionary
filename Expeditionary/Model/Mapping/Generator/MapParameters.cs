namespace Expeditionary.Model.Mapping.Generator
{
    public class MapParameters
    {
        public TerrainParameters Terrain { get; set; } = new();
        public CityParameters Cities { get; set; } = new();
    }
}
