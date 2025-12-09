namespace Expeditionary.Model.Galaxies
{
    public record class Galaxy
    {
        public float Scale { get; set; } = 1f;
        public List<Sector> Sectors { get; set; } = new();
    }
}
