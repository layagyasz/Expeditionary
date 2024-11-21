using Cardamom;
using Cardamom.Collections;
using Cardamom.Json;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Expeditionary.Model.Combat.Units
{
    [JsonConverter(typeof(BuilderJsonConverter))]
    [BuilderClass(typeof(UnitTypeDefinition))]
    public class UnitType : IKeyed
    {
        public string Key
        {
            get => Definition.Key; 
            set => Definition.Key = value;
        }
        public string Name => Definition.Name;
        public string? Symbol => Definition.Symbol;
        public UnitTypeDefinition Definition { get; }
        public ImmutableList<UnitWeaponUsage> Weapons { get; }
        public UnitDefense Defense { get; }
        public Movement Movement { get; }
        public UnitCapabilities Capabilities { get; }
        public UnitIntrinsics Intrinsics { get; }
        public float Speed { get; }

        public UnitType(
            UnitTypeDefinition definition,
            IEnumerable<UnitWeaponUsage> weapons,
            UnitDefense defense,
            Movement movement,
            UnitCapabilities capabilities,
            UnitIntrinsics intrinsics)
        {
            Definition = definition;
            Weapons = ImmutableList.CreateRange(weapons);
            Defense = defense;
            Movement = movement;
            Capabilities = capabilities;
            Intrinsics = intrinsics;
            Speed = intrinsics.Power.GetValue() 
                / (intrinsics.Mass.GetValue() + weapons.Sum(x => x.Weapon.Mass.GetValue()));
        }

        public EnumSet<UnitTag> GetTags()
        {
            return Definition.GetTags();
        }

        public bool Validate()
        {
            return Speed >= 1 
                && Intrinsics.Space.Available.GetValue() 
                    >= (Intrinsics.Space.Used.GetValue() + Weapons.Sum(x => x.Weapon.Size.GetValue()));
        }
    }
}
