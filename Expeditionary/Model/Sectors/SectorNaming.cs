using Cardamom;

namespace Expeditionary.Model.Sectors
{
    public class SectorNaming : IKeyed
    {
        public string Key { get; set; } = string.Empty;
        public List<string> StarPrefixes { get; set; } = new();
        public List<SectorName> Sectors { get; set; } = new();

        public string Name(PlanetKey key)
        {
            return $"{StarPrefixes[key.System]} {Sectors[key.Sector].StarName} {key.Planet}";
        }
    }
}
