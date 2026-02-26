using System.Text.Json.Serialization;

namespace Expeditionary.Model.Mapping
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum StructureType
    {
        None,

        Agricultural,
        Mining,
        Residential,
        Commercial,
        Industrial,
    }
}
