using Cardamom;
using Cardamom.Collections;
using Expeditionary.Model.Combat.Units;

namespace Expeditionary.Model.Combat.Formations
{
    public record class FormationSlot : IKeyed
    {
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Number { get; set; }
        public UnitRole Role { get; set; }
        public EnumSet<UnitTag> RequiredTags { get; set; } = new();
        public EnumSet<UnitTag> ExcludedTags { get; set; } = new();
    }
}
