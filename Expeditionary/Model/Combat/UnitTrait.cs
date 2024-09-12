using Cardamom;

namespace Expeditionary.Model.Combat
{
    public class UnitTrait : IKeyed
    {
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Family { get; set; } = string.Empty;
        public int Cost { get; set; }
        public int Level { get; set; }
        public Dictionary<string, UnitModifier> Modifiers { get; set; } = new();
    }
}
