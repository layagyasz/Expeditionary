using Expeditionary.Model.Formations;

namespace Expeditionary.Model.Matches.Assets
{
    public class MatchFormation : BaseFormation<MatchFormation, MatchDiad, MatchUnit>
    {
        public int Id { get; }
        public int InstanceId { get; }
        public Player Player { get; }

        private MatchFormation(
            int id,
            Player player,
            string name,
            FormationRole role,
            int echelon,
            IEnumerable<MatchFormation> componentFormations,
            IEnumerable<MatchDiad> diads)
            : base(name, role, echelon, componentFormations, diads)
        {
            Id = id;
            Player = player;
        }

        public static MatchFormation From(TemplateFormation template, Player player, IIdGenerator idGenerator)
        {
            return new MatchFormation(
                idGenerator.Next(),
                player,
                template.Name, 
                template.Role,
                template.Echelon,
                template.ComponentFormations.Select(componentTemplate => From(componentTemplate, player, idGenerator)), 
                template.Diads.Select(diadTemplate => MatchDiad.From(diadTemplate, player, idGenerator)));
        }

        public void AddComponent(MatchFormation formation)
        {
            _componentFormations.Add(formation);
        }

        public AssetValue GetAliveUnitQuantity()
        {
            return _componentFormations.Select(x => x.GetAliveUnitQuantity()).Concat(
                _diads.Select(x => x.Unit.Value))
                .Aggregate(AssetValue.None, (x, y) => x + y);
        }
    }
}
