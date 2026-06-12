using Expeditionary.Model.Formations;
using Expeditionary.Model.Instances;
using Expeditionary.Model.Units;

namespace Expeditionary.Model.Matches.Assets
{
    public class MatchFormation : BaseFormation<MatchFormation, MatchUnit>
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
            IEnumerable<FormationDiad<MatchUnit>> diads)
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
                instance.Diads.Select(diadInstance => From(diadInstance, player, idGenerator)));
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
                template.Diads.Select(diadTemplate => From(diadTemplate, player, idGenerator)));
        }

        public AssetValue GetUnitValue(Func<MatchUnit, bool> predicate)
        {
            return GetUnits().Where(predicate).Select(unit => unit.Value).Aggregate(AssetValue.None, (x, y) => x + y);
        }


        private static FormationDiad<MatchUnit> From(
            FormationDiad<InstanceUnit> instance, MatchPlayer player, IIdGenerator idGenerator)
        {
            return new(
                instance.Role,
                ToMatchUnit(instance.Unit, idGenerator, player),
                instance.Transport == null ? null : ToMatchUnit(instance.Transport, idGenerator, player));
        }

        private static FormationDiad<MatchUnit> From(
            FormationDiad<UnitType> template, MatchPlayer player, IIdGenerator idGenerator)
        {
            return new(
                template.Role,
                new(idGenerator.Next(), Constants.NoInstanceId, player, template.Unit),
                template.Transport == null
                    ? null
                    : new(idGenerator.Next(), Constants.NoInstanceId, player, template.Transport));
        }

        private static MatchUnit ToMatchUnit(InstanceUnit unit, IIdGenerator idGenerator, MatchPlayer player)
        {
            return new(idGenerator.Next(), unit.Id, player, unit.Type, unit.Number);
        }
    }
}
