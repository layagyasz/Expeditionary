using System.Text.Json.Serialization;

namespace Expeditionary.Model.Mapping
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EdgeType
    {
        None,
        River,
        Road
    }
}
