using Expeditionary.Model.Formations;

namespace Expeditionary.Model.Instances
{
    public record class InstanceDiad : BaseDiad<InstanceUnit>
    {
        private InstanceDiad(FormationRole role, InstanceUnit unit, InstanceUnit? transport)
            : base(role, unit, transport) { }

        public static InstanceDiad From(TemplateDiad template, IIdGenerator idGenerator)
        {
            return new(
                template.Role,
                new(idGenerator.Next(), template.Unit),
                template.Transport == null ? null : new(idGenerator.Next(), template.Transport));
        }
    }
}
