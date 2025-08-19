using System.Text.Json.Serialization;

namespace Expeditionary.Model.Combat
{
    [Flags]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CombatCondition
    {
        None = 0,
        
        Direct = 1,
        Indirect = 2,

        Close = 4,
        Dark = 8,
        Melee = 16,
        Ranged = 32,
        Urban = 64
    }
}
