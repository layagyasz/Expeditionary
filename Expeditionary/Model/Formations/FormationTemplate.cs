using Cardamom.Json.Collections;
using Expeditionary.Model.Units;
using System.Text.Json.Serialization;

namespace Expeditionary.Model.Formations
{
    public record class FormationTemplate
    {
        public string Name { get; }
        public List<FormationTemplate> ComponentFormations { get; }

        [JsonConverter(typeof(ReferenceCollectionJsonConverter))]
        public List<UnitType> Units { get; }

        public FormationTemplate(string name, List<FormationTemplate> componentFormations, List<UnitType> units)
        {
            Name = name;
            ComponentFormations = componentFormations;
            Units = units;
        }

        public IEnumerable<UnitType> GetUnitTypes()
        {
            return Enumerable.Concat(Units, ComponentFormations.SelectMany(x => x.GetUnitTypes()));
        }
    }
}
