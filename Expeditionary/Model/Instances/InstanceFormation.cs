using Expeditionary.Model.Formations;
using Expeditionary.Model.Units;

namespace Expeditionary.Model.Instances
{
    public class InstanceFormation : BaseFormation<InstanceFormation, InstanceUnit>
    {
        public int Id { get; }

        private InstanceFormation(
            int id, 
            string name,
            FormationRole role,
            int echelon,
            IEnumerable<InstanceFormation> componentFormations,
            IEnumerable<FormationDiad<InstanceUnit>> diads)
            : base(name, role, echelon, componentFormations, diads)
        {
            Id = id;
        }

        public static InstanceFormation From(TemplateFormation template, IIdGenerator idGenerator)
        {
            return new(
                idGenerator.Next(), 
                template.Name, 
                template.Role,
                template.Echelon, 
                template.ComponentFormations.Select(componentTemplate => From(componentTemplate, idGenerator)),
                template.Diads.Select(diadTemplate => From(diadTemplate, idGenerator)));
        }

        public InstanceUnit? GetUnit(int id)
        {
            return GetUnits().First(unit => unit.Id == id);
        }

        private static FormationDiad<InstanceUnit> From(FormationDiad<UnitType> template, IIdGenerator idGenerator)
        {
            return new(
                template.Role,
                new(idGenerator.Next(), template.Unit),
                template.Transport == null ? null : new(idGenerator.Next(), template.Transport));
        }
    }
}
