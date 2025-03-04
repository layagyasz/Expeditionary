using Cardamom.Json.Collections;
using Expeditionary.Model.Units;
using System.Text.Json.Serialization;

namespace Expeditionary.Model.Formations
{
    public record class FormationTemplate
    {
        public record class UnitTypeAndRole(UnitType UnitType, FormationRole Role);

        public string Name { get; }
        public FormationRole Role { get; }
        public int Echelon { get; }
        public List<FormationTemplate> ComponentFormations => _componentFormations;

        [JsonConverter(typeof(ReferenceCollectionJsonConverter))]
        public IEnumerable<UnitTypeAndRole> UnitTypesAndRoles => _unitTypesAndRoles;

        private readonly List<UnitTypeAndRole> _unitTypesAndRoles;
        private readonly List<FormationTemplate> _componentFormations;

        public FormationTemplate(
            string name,
            FormationRole role,
            int echelon,
            IEnumerable<FormationTemplate> componentFormations,
            IEnumerable<UnitTypeAndRole> unitTypesAndRoles)
        {
            Name = name;
            Role = role;
            Echelon = echelon;
            _componentFormations = componentFormations.ToList();
            _unitTypesAndRoles = unitTypesAndRoles.ToList();
        }

        public IEnumerable<UnitTypeAndRole> GetUnitTypesAndRoles()
        {
            return Enumerable.Concat(UnitTypesAndRoles, ComponentFormations.SelectMany(x => x.GetUnitTypesAndRoles()));
        }
    }
}
