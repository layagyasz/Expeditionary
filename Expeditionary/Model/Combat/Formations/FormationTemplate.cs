using Cardamom.Json.Collections;
using Expeditionary.Model.Combat.Units;
using System.Text.Json.Serialization;

namespace Expeditionary.Model.Combat.Formations
{
    public record class FormationTemplate
    {
        public int Id { get; }
        public string Name { get; }
        public List<FormationTemplate> ComponentFormations { get; }

        [JsonConverter(typeof(ReferenceCollectionJsonConverter))]
        public List<UnitType> Units { get; }

        public FormationTemplate(
            int id, string name, List<FormationTemplate> componentFormations, List<UnitType> units)
        {
            Id = id;
            Name = name;
            ComponentFormations = componentFormations;
            Units = units;
        }
    }
}
