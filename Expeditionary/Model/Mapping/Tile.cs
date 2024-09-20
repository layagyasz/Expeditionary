namespace Expeditionary.Model.Mapping
{
    public class Tile
    {
        public float Elevation { get; set; }
        public float Slope { get; set; }
        public Terrain Terrain { get; set; } = new();
        public Structure Structure { get; set; }
    }
}
