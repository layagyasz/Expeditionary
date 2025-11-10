using Cardamom;
using Expeditionary.Model.Mapping.Environments;

namespace Expeditionary.Model.Galaxies
{
    public class SectorNaming : IKeyed
    {
        public string Key { get; set; } = string.Empty;
        public required List<string> StarPrefixes { get; set; }
        public required List<SectorName> Sectors { get; set; }

        public string Name(MapEnvironmentDefinition environment)
        {
            if (environment.Name != null)
            {
                return environment.Name;
            }
            var key = environment.Location;
            return $"{Capitalize(StarPrefixes[key.System])} {Capitalize(Sectors[key.Sector].StarName)} {key.Planet}";
        }

        private static string Capitalize(string value)
        {
            if (value.Any())
            {
                return char.ToUpper(value[0]) + value.Substring(1);
            }
            return value;
        }
    }
}
