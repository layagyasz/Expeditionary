using Expeditionary.Model.Units;

namespace Expeditionary.Model.Formations
{
    public class TemplateFormation : BaseFormation<TemplateFormation, UnitType> 
    {
        public AssetValue Value { get; }

        public TemplateFormation(
            string name,
            FormationRole role,
            int echelon,
            IEnumerable<TemplateFormation> componentFormations,
            IEnumerable<FormationDiad<UnitType>> diads) 
            : base(name, role, echelon, componentFormations, diads) 
        {
            Value = 
                GetUnits().Select(unit => new AssetValue(1, unit.Points)).Aggregate(AssetValue.None, (x, y) => x + y);
        }
    }
}
