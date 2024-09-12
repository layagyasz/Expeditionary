using Cardamom;

namespace Expeditionary.Model.Combat
{
    public class UnitTraitSet : IKeyed
    {
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public List<UnitTrait> Traits { get; set; } = new();
    }
}
