using System.Text.Json.Serialization;

namespace Expeditionary.Model.Combat
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
