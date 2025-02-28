using Cardamom.Json;

namespace Expeditionary.Model.Units
{
    [BuilderClass(typeof(UnitWeaponUsageDefinition))]
    public record class UnitWeaponUsage(int Number, UnitWeapon Weapon);
}
