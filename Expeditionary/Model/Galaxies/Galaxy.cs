namespace Expeditionary.Model.Galaxies
{
    public record class Galaxy
    {
        public List<Sector> Sectors { get; set; } = new();
    }
}
