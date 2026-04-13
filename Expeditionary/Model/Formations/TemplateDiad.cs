using Expeditionary.Model.Units;

namespace Expeditionary.Model.Formations
{
    public record class TemplateDiad : BaseDiad<UnitType>
    {
        public TemplateDiad(FormationRole role, UnitType unit, UnitType? transport) 
            : base(role, unit, transport) { }
    }
}
