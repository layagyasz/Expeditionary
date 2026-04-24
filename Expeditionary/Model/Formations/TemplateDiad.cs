using Expeditionary.Model.Units;

namespace Expeditionary.Model.Formations
{
    public record class TemplateDiad : BaseDiad<UnitType>
    {
        public AssetValue Value => new AssetValue(1, Unit.Points)
            + (Transport == null ? AssetValue.None : new(1, Transport.Points));

        public TemplateDiad(FormationRole role, UnitType unit, UnitType? transport) 
            : base(role, unit, transport) { }
    }
}
