using Cardamom;
using Cardamom.Collections;
using System.Collections.Immutable;

namespace Expeditionary.Model.Combat.Units
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
        }

        public string Key { get; set; }
        public string Name { get; }
        public UnitWeaponDefinition Definition { get; }
        public ImmutableList<Mode> Modes { get; }

        public UnitWeapon(string key, string name, UnitWeaponDefinition definition, IEnumerable<Mode> modes)
        {
            Key = key;
            Name = name;
            Definition = definition;
            Modes = modes.ToImmutableList();
        }

        public EnumSet<UnitTag> GetTags()
        {
            return Enumerable.Concat(Definition.Traits, Definition.Modes.SelectMany(x => x.Traits))
                .SelectMany(x => x.Tags)
                .ToEnumSet();
        }
    }
}
