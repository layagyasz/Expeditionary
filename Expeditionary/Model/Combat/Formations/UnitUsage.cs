using Cardamom.Json;
using Expeditionary.Model.Combat.Units;
using System.Text.Json.Serialization;

namespace Expeditionary.Model.Combat.Formations
{
    public record class UnitUsage
    {
        public UnitRole Role { get; set; }

        [JsonConverter(typeof(ReferenceJsonConverter))]
        public UnitType? Type { get; set; }
    }
}
