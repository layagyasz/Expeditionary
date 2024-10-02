using Cardamom;

namespace Expeditionary.Model.Factions
{
    public class Faction : IKeyed
    {
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public ColorScheme ColorScheme { get; set; } = new();
    }
}
