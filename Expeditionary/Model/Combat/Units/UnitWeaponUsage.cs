using Cardamom.Json;
using System.Text.Json.Serialization;

namespace Expeditionary.Model.Combat.Units
{
    [JsonConverter(typeof(BuilderJsonConverter))]
    [BuilderClass(typeof(UnitWeaponUsageDefinition))]
    public record class UnitWeaponUsage(bool IsDistributed, int Number, UnitWeapon Weapon);
}
