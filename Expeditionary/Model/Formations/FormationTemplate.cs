using Cardamom.Json.Collections;
using Expeditionary.Model.Units;
using System.Text.Json.Serialization;

namespace Expeditionary.Model.Formations
{
    public record class FormationTemplate
    {
        public string Name { get; }
        public FormationRole Role { get; }
        public int Echelon { get; }

        [JsonConverter(typeof(ReferenceCollectionJsonConverter))]
        public IEnumerable<(UnitType, FormationRole)> UnitTypesAndRoles => _unitTypesAndRoles;
        public List<FormationTemplate> ComponentFormations => _componentFormations;

        private readonly List<(UnitType, FormationRole)> _unitTypesAndRoles;
        private readonly List<FormationTemplate> _componentFormations;

        public FormationTemplate(
            string name,
            FormationRole role,
            int echelon,
            IEnumerable<FormationTemplate> componentFormations,
            IEnumerable<(UnitType, FormationRole)> unitTypesAndRoles)
        {
            Name = name;
            Role = role;
            Echelon = echelon;
            _componentFormations = componentFormations.ToList();
            _unitTypesAndRoles = unitTypesAndRoles.ToList();
        }

        public IEnumerable<(UnitType, FormationRole)> GetUnitTypesAndRoles()
        {
            return Enumerable.Concat(UnitTypesAndRoles, ComponentFormations.SelectMany(x => x.GetUnitTypesAndRoles()));
        }

        public Formation Materialize(Player player, IIdGenerator idGenerator)
        {
            return new(
                player,
                Name, 
                Role,
                Echelon,
                UnitTypesAndRoles.Select(x => (new Unit(idGenerator.Next(), player, x.Item1), x.Item2)),
                ComponentFormations.Select(x => x.Materialize(player, idGenerator)));
        }
    }
}
