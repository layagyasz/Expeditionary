using Cardamom;
using Expeditionary.Model.Combat.Formations;

namespace Expeditionary.Model.Factions
{
    public class FactionFormationConfiguration : IKeyed
    {
        public string Key { get; set; } = string.Empty;
        public string Faction { get; set; } = string.Empty;
        public List<UnitUsage> Units { get; set; } = new();
    }
}
