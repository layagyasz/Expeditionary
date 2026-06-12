using Expeditionary.Model.Formations;
using Expeditionary.Model.Matches.Assets;

namespace Expeditionary.Model.Matches.Reporting
{
    public class FormationReport : BaseFormation<FormationReport, UnitReport>
    {
        public FormationReport(
            string name,
            FormationRole role,
            int echelon,
            IEnumerable<FormationReport> componentFormations,
            IEnumerable<FormationDiad<UnitReport>> diads)
            : base(name, role, echelon, componentFormations, diads) { }

        public static FormationReport Generate(MatchFormation formation)
        {
            return new(
                formation.Name,
                formation.Role, 
                formation.Echelon, 
                formation.ComponentFormations.Select(Generate),
                formation.Diads.Select(Generate));
        }

        private static FormationDiad<UnitReport> Generate(FormationDiad<MatchUnit> diad)
        {
            return new(
                diad.Role,
                UnitReport.Generate(diad.Unit), 
                diad.Transport == null ? null : UnitReport.Generate(diad.Transport));
        }
    }
}
