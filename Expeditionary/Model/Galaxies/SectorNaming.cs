using Cardamom;
using Expeditionary.Model.Mapping.Environments;

namespace Expeditionary.Model.Galaxies
{
    public class SectorNaming : IKeyed
    {
        public string Key { get; set; } = string.Empty;
        public required List<string> StarPrefixes { get; set; }
        public required List<SectorName> Sectors { get; set; }

        public string Name(MapEnvironment environment)
        {
            if (environment.Name != null)
            {
                return environment.Name;
            }
            var key = environment.Location;
            return $"{StarPrefixes[key.System]} {Sectors[key.Sector].StarName} {key.Planet}";
        }
    }
}
