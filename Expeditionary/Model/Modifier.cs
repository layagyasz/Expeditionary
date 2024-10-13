using Expeditionary.Json;
using System.Text.Json.Serialization;

namespace Expeditionary.Model
{
    [JsonConverter(typeof(ModifierJsonConverter))]
    public struct Modifier
    {
        public static readonly Modifier None = new(1, 0);

        public float Multiplier { get; set; } = 1f;
        public float Bonus { get; set; } = 0f;

        public Modifier() { }

        public Modifier(float multiplier, float bonus)
        {
            Multiplier = multiplier;
            Bonus = bonus;
        }

        public float GetValue()
        {
            return Bonus * Multiplier;
        }

        public static Modifier operator +(Modifier left, Modifier right)
        {
            return new(left.Multiplier * right.Multiplier, left.Bonus + right.Bonus);
        }
    }
}
