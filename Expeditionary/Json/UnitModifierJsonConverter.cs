using Expeditionary.Model.Combat;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Expeditionary.Json
{
    public class UnitModifierJsonConverter : JsonConverter<UnitModifier>
    {
        public override UnitModifier Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string stringValue = reader.GetString()!;
            if (stringValue[0] != '+' || !stringValue.Contains('x'))
            {
                throw new JsonException($"Improperly formatted modifier '{stringValue}'.");
            }
            string[] tokens = stringValue.Split('x');
            return new()
            {
                Bonus = float.Parse(tokens[0].Substring(1)),
                Multiplier = float.Parse(tokens[1])
            };
        }

        public override void Write(Utf8JsonWriter writer, UnitModifier @object, JsonSerializerOptions options)
        {
            writer.WriteStringValue(string.Format("+{0}x{1}", @object.Bonus, @object.Multiplier));
        }
    }
}
