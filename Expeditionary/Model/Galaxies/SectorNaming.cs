using Cardamom;
using Expeditionary.Model.Mapping.Environments;

namespace Expeditionary.Model.Galaxies
{
    public class SectorNaming : IKeyed
    {
        public string Key { get; set; } = string.Empty;
        public List<string> StarPrefixes { get; set; } = new();
        public List<SectorName> Sectors { get; set; } = new();

        public string Name(MapEnvironmentName name)
        {
            if (name.KeyedName != null)
            {
                var key = name.KeyedName.Value;
                return $"{StarPrefixes[key.System]} {Sectors[key.Sector].StarName} {key.Planet}";
            }
            return name.FixedName!;
        }
    }
}
