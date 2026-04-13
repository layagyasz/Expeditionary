using Expeditionary.Model.Formations;

namespace Expeditionary.Model.Instances
{
    public class InstanceFormation : BaseFormation<InstanceFormation, InstanceDiad, InstanceUnit>
    {
        public int Id { get; }

        private InstanceFormation(
            int id, 
            string name,
            FormationRole role,
            int echelon,
            IEnumerable<InstanceFormation> componentFormations,
            IEnumerable<InstanceDiad> diads)
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
                template.Diads.Select(diadTemplate => InstanceDiad.From(diadTemplate, idGenerator)));
        }
    }
}
