namespace Expeditionary.Model.Galaxies
{
    public record class SectorName
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string StarName { get; set; } = string.Empty;
    }
}
