using Expeditionary.Model.Formations;
using Expeditionary.Model.Instances;

namespace Expeditionary.Model.Matches.Assets
{
    public record class MatchDiad : BaseDiad<MatchUnit>
    {
        private MatchDiad(FormationRole role, MatchUnit unit, MatchUnit? transport)
        : base(role, unit, transport) { }

        public static MatchDiad From(InstanceDiad instance, MatchPlayer player, IIdGenerator idGenerator)
        {
            return new(
                instance.Role,
                new(idGenerator.Next(), instance.Unit.Id, player, instance.Unit.Type),
                instance.Transport == null
                    ? null
                    : new(idGenerator.Next(), instance.Transport.Id, player, instance.Transport.Type));
        }

        public static MatchDiad From(TemplateDiad template, MatchPlayer player, IIdGenerator idGenerator)
        {
            return new(
                template.Role,
                new(idGenerator.Next(), Constants.NoInstanceId, player, template.Unit),
                template.Transport == null 
                    ? null 
                    : new(idGenerator.Next(), Constants.NoInstanceId, player, template.Transport));
        }
    }
}
