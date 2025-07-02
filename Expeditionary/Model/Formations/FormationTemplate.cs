using Cardamom.Json.Collections;
using Expeditionary.Model.Units;
using System.Text.Json.Serialization;

namespace Expeditionary.Model.Formations
{
    public record class FormationTemplate
    {
        public record class Diad(FormationRole Role, UnitType UnitType, UnitType? TransportType)
        {
            public Formation.Diad Materialize(Player player, IIdGenerator idGenerator)
            {
                return new(
                    Role,
                    new Unit(idGenerator.Next(), player, UnitType), TransportType == null ? null : 
                    new Unit(idGenerator.Next(), player, TransportType));
            }
        }

        public string Name { get; }
        public FormationRole Role { get; }
        public int Echelon { get; }

        [JsonConverter(typeof(ReferenceCollectionJsonConverter))]
        public IEnumerable<Diad> Diads => _diads;
        public List<FormationTemplate> ComponentFormations => _componentFormations;

        private readonly List<Diad> _diads;
        private readonly List<FormationTemplate> _componentFormations;

        public FormationTemplate(
            string name,
            FormationRole role,
            int echelon,
            IEnumerable<FormationTemplate> componentFormations,
            IEnumerable<Diad> unitTypesAndRoles)
        {
            Name = name;
            Role = role;
            Echelon = echelon;
            _componentFormations = componentFormations.ToList();
            _diads = unitTypesAndRoles.ToList();
        }

        public IEnumerable<Diad> GetDiads()
        {
            return Enumerable.Concat(Diads, ComponentFormations.SelectMany(x => x.GetDiads()));
        }

        public Formation Materialize(Player player, IIdGenerator idGenerator)
        {
            return new(
                player,
                Name, 
                Role,
                Echelon,
                Diads.Select(x => x.Materialize(player, idGenerator)),
                ComponentFormations.Select(x => x.Materialize(player, idGenerator)));
        }
    }
}
