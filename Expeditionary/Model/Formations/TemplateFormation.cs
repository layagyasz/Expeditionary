using Expeditionary.Model.Units;

namespace Expeditionary.Model.Formations
{
    public class TemplateFormation : BaseFormation<TemplateFormation, TemplateDiad, UnitType> 
    {
        public TemplateFormation(
            string name,
            FormationRole role,
            int echelon,
            IEnumerable<TemplateFormation> componentFormations,
            IEnumerable<TemplateDiad> diads) 
            : base(name, role, echelon, componentFormations, diads) { }
    }
}
