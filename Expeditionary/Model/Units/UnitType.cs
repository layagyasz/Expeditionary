using Cardamom;
using Cardamom.Json;
using System.Collections.Immutable;

namespace Expeditionary.Model.Units
{
    [BuilderClass(typeof(UnitTypeDefinition))]
    public class UnitType : IKeyed
    {
        public string Key { get; set; }
        public string Name { get; }
        public string? Symbol { get; }
        public ImmutableList<UnitTag> Tags { get; }
        public ImmutableList<UnitWeaponUsage> Weapons { get; }
        public UnitDefense Defense { get; }
        public Movement Movement { get; }
        public UnitCapabilities Capabilities { get; }
        public UnitIntrinsics Intrinsics { get; }
        public int Points { get; }
        public float Speed { get; }

        public UnitType(
            string key,
            string name,
            string? symbol,
            IEnumerable<UnitTag> tags,
            IEnumerable<UnitWeaponUsage> weapons,
            UnitDefense defense,
            Movement movement,
            UnitCapabilities capabilities,
            UnitIntrinsics intrinsics,
            int points)
        {
            Key = key;
            Name = name;
            Symbol = symbol;
            Tags = tags.Distinct().ToImmutableList();
            Weapons = ImmutableList.CreateRange(weapons);
            Defense = defense;
            Movement = movement;
            Capabilities = capabilities;
            Intrinsics = intrinsics;
            Points = points;
            Speed = intrinsics.Power.GetValue() / intrinsics.Mass;
        }

        public bool Validate()
        {
            return Intrinsics.Space.Available >= Intrinsics.Space.Used;
        }
    }
}
