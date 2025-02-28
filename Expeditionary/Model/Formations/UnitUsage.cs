using Cardamom.Json;
using Expeditionary.Model.Units;
using System.Text.Json.Serialization;

namespace Expeditionary.Model.Formations
{
    public record class UnitUsage
    {
        public UnitRole Role { get; set; }

        [JsonConverter(typeof(ReferenceJsonConverter))]
        public UnitType? Type { get; set; }
    }
}
