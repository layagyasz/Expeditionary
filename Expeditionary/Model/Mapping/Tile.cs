namespace Expeditionary.Model.Mapping
{
    public class Tile
    {
        public float Elevation { get; set; }
        public float Slope { get; set; }
        public float Heat { get; set; }
        public float Moisture { get; set; }
        public Terrain Terrain { get; set; } = new();
        public Structure Structure { get; set; }
    }
}
