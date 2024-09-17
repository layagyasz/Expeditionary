using System.Text.Json.Serialization;

namespace Expeditionary.model.combat
{
    [Flags]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CombatCondition
    {
        None, 

        Close,
        Dark,
        Melee,
        Ranged,
        Urban
    }
}
