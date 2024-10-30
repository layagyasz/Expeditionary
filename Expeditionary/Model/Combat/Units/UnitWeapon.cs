﻿using Cardamom;
using Cardamom.Collections;
using Cardamom.Json;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Expeditionary.Model.Combat.Units
{
    [JsonConverter(typeof(BuilderJsonConverter))]
    [BuilderClass(typeof(UnitWeaponDefinition))]
    public record class UnitWeapon : IKeyed
    {
        public record class Mode
        {
            public CombatCondition Condition { get; init; }
            public Modifier Volume { get; init; }
            public Modifier Range { get; init; }
            public Modifier Accuracy { get; init; }
            public Modifier Tracking { get; init; }
            public Modifier Penetration { get; init; }
            public Modifier Lethality { get; init; }
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
