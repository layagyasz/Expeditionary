using Cardamom.Json;

namespace Expeditionary.Model.Matches.Assets
{
    [BuilderClass(typeof(UnitWeaponUsageDefinition))]
    public record class UnitWeaponUsage(int Number, UnitWeapon Weapon);
}
