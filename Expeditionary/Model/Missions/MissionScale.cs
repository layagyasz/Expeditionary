using System.Text.Json.Serialization;

namespace Expeditionary.Model.Missions
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum MissionScale
    {
        Small,
        Medium,
        Large
    }
}
