using Cardamom;
using Expeditionary.Model.Mapping.Environments;
using System.Text;

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
            return $"{Capitalize(StarPrefixes[key.System])} " 
                + $"{Capitalize(Sectors[key.Sector].StarName)} "
                + $"{ToRoman(key.Planet)}";
        }

        private static string Capitalize(string value)
        {
            if (value.Any())
            {
                return char.ToUpper(value[0]) + value.Substring(1);
            }
            return value;
        }

        private static readonly int[] s_RomanValues = { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };
        private static readonly string[] s_RomanLiterals =
            { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };
        private static string ToRoman(long value)
        {
            StringBuilder result = new();
            for (int i = 0; i < s_RomanValues.Length; i++)
            {
                while (value >= s_RomanValues[i])
                {
                    value -= s_RomanValues[i];
                    result.Append(s_RomanLiterals[i]);
                }
            }
            return result.ToString();
        }
    }
}
