using Expeditionary.Model.Formations;
using Expeditionary.Model.Instances;

namespace Expeditionary.Model.Matches.Assets
{
    public class MatchFormation : BaseFormation<MatchFormation, MatchDiad, MatchUnit>
    {
        public int Id { get; }
        public int InstanceId { get; }
        public MatchPlayer Player { get; }

        private MatchFormation(
            int id,
            int instanceId,
            MatchPlayer player,
            string name,
            FormationRole role,
            int echelon,
            IEnumerable<MatchFormation> componentFormations,
            IEnumerable<MatchDiad> diads)
            : base(name, role, echelon, componentFormations, diads)
        {
            Id = id;
            InstanceId = instanceId;
            Player = player;
        }

        public static MatchFormation From(InstanceFormation instance, MatchPlayer player, IIdGenerator idGenerator)
        {
            return new MatchFormation(
                idGenerator.Next(),
                instance.Id,
                player,
                instance.Name,
                instance.Role,
                instance.Echelon,
                instance.ComponentFormations.Select(componentInstance => From(componentInstance, player, idGenerator)),
                instance.Diads.Select(diadInstance => MatchDiad.From(diadInstance, player, idGenerator)));
        }

        public static MatchFormation From(TemplateFormation template, MatchPlayer player, IIdGenerator idGenerator)
        {
            return new MatchFormation(
                idGenerator.Next(),
                Constants.NoInstanceId,
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

        public AssetValue GetUnitValue(Func<MatchUnit, bool> predicate)
        {
            return GetUnits().Where(predicate).Select(unit => unit.Value).Aggregate(AssetValue.None, (x, y) => x + y);
        }
    }
}
