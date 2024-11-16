using Expeditionary.Json;
using System.Text.Json.Serialization;

namespace Expeditionary.Model
{
    [JsonConverter(typeof(ModifierJsonConverter))]
    public record struct Modifier
    {
        public static readonly Modifier None = new();

        public float Multiplier { get; set; } = 0f;
        public float Bonus { get; set; } = 0f;

        public Modifier() { }

        public Modifier(float multiplier, float bonus)
        {
            Multiplier = multiplier;
            Bonus = bonus;
        }

        public float GetValue()
        {
            return Bonus * (1 + Multiplier);
        }

        public static Modifier Add(Modifier left, Modifier right)
        {
            return left + right;
        }

        public static Modifier operator +(Modifier left, Modifier right)
        {
            return new(left.Multiplier + right.Multiplier, left.Bonus + right.Bonus);
        }
    }
}
