using Expeditionary.Model.Formations;

namespace Expeditionary.Model.Matches.Assets
{
    public record class MatchDiad : BaseDiad<MatchUnit>
    {
        private MatchDiad(FormationRole role, MatchUnit unit, MatchUnit? transport)
        : base(role, unit, transport) { }

        public static MatchDiad From(TemplateDiad template, Player player, IIdGenerator idGenerator)
        {
            return new(
                template.Role,
                new(idGenerator.Next(), player, template.Unit),
                template.Transport == null ? null : new(idGenerator.Next(), player, template.Transport));
        }
    }
}
