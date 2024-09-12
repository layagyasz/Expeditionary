using Expeditionary.Json;
using System.Text.Json.Serialization;

namespace Expeditionary.Model.Combat
{
    [JsonConverter(typeof(UnitModifierJsonConverter))]
    public struct UnitModifier
    {
        public float Multiplier { get; set; } = 1f;
        public float Bonus { get; set; } = 0f;

        public UnitModifier() { }

        public UnitModifier(float multiplier, float bonus)
        {
            Multiplier = multiplier;
            Bonus = bonus;
        }

        public float GetValue()
        {
            return Bonus * Multiplier;
        }

        public static UnitModifier operator +(UnitModifier left, UnitModifier right)
        {
            return new(left.Multiplier * right.Multiplier, left.Bonus + right.Bonus);
        }
    }
}
