using Cardamom;
using Cardamom.Collections;
using Expeditionary.Model.Combat;
using System.Collections.Immutable;

namespace Expeditionary.Model.Units
{
    public record class UnitWeapon : IKeyed
    {
        public record class Mode
        {
            public CombatCondition Condition { get; init; }
            public Modifier Volume { get; init; }
            public UnitWeaponRange Range { get; init; }
            public Modifier Accuracy { get; init; }
            public Modifier Tracking { get; init; }
            public Modifier Penetration { get; init; }
            public Modifier Lethality { get; init; }
            public EnumMap<UnitDetectionBand, Modifier> Signature { get; init; } = new();

            public bool IsIndirect()
            {
                return Condition.HasFlag(CombatCondition.Indirect);
            }
        }

        public string Key { get; set; }
        public string Name { get; }
        public ImmutableList<UnitTag> Tags { get; }
        public ImmutableList<Mode> Modes { get; }
        public Modifier Size { get; }
        public Modifier Mass { get; }
        public int Points { get; }

        public UnitWeapon(
            string key, 
            string name,
            IEnumerable<UnitTag> tags,
            IEnumerable<Mode> modes, 
            Modifier mass,
            Modifier size,
            int points)
        {
            Key = key;
            Name = name;
            Tags = tags.Distinct().ToImmutableList();
            Modes = modes.ToImmutableList();
            Mass = mass;
            Size = size;
            Points = points;
        }
    }
}
