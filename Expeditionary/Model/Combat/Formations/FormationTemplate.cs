using Cardamom.Json.Collections;
using System.Text.Json.Serialization;

namespace Expeditionary.Model.Combat.Formations
{
    public record class FormationTemplate
    {
        [JsonConverter(typeof(ReferenceCollectionJsonConverter))]
        public List<FormationTemplate> ComponentFormations { get; set; } = new();

        public List<FormationSlot> Slots { get; set; } = new();
    }
}
