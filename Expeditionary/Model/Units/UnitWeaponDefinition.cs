using Cardamom;
using Cardamom.Json;
using Cardamom.Json.Collections;
using Expeditionary.Model.Combat;
using System.Text.Json.Serialization;

namespace Expeditionary.Model.Units
{
    public record class UnitWeaponDefinition : IKeyed
    {
        public record class Mode
        {
            [JsonConverter(typeof(FlagJsonConverter<CombatCondition>))]
            public CombatCondition Condition { get; set; }

            [JsonConverter(typeof(ReferenceCollectionJsonConverter))]
            public List<UnitTrait> Traits { get; set; } = new();
        }

        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        [JsonConverter(typeof(ReferenceCollectionJsonConverter))]
        public List<UnitTrait> Traits { get; set; } = new();
        public List<Mode> Modes { get; set; } = new();

        public UnitWeapon Build(IEnumerable<UnitTrait> extraTraits)
        {
            var baseAttributes = UnitTrait.Combine(Enumerable.Concat(extraTraits, Traits));
            return new(
                Key,
                Name,
                this,
                Modes.Select(x => BuildMode(x, Enumerable.Concat(extraTraits, Traits))),
                UnitTrait.GetOrDefault(baseAttributes, "mass", Modifier.None), 
                UnitTrait.GetOrDefault(baseAttributes, "size", Modifier.None));
        }

        private static UnitWeapon.Mode BuildMode(Mode definition, IEnumerable<UnitTrait> baseTraits)
        {
            var attributes = UnitTrait.Combine(Enumerable.Concat(definition.Traits, baseTraits));
            return new()
            {
                Condition = definition.Condition,
                Volume = UnitTrait.GetOrDefault(attributes, "volume", Modifier.None),
                Range = BuildRange("range", attributes),
                Accuracy = UnitTrait.GetOrDefault(attributes, "accuracy", Modifier.None),
                Tracking = UnitTrait.GetOrDefault(attributes, "tracking", Modifier.None),
                Penetration = UnitTrait.GetOrDefault(attributes, "penetration", Modifier.None),
                Lethality = UnitTrait.GetOrDefault(attributes, "lethality", Modifier.None),
                Signature = UnitTrait.GetMap<UnitDetectionBand, Modifier>(
                    "signature", x => UnitTrait.GetOrDefault(attributes, x, Modifier.None))
            };
        }

        private static UnitWeaponRange BuildRange(string prefix, IDictionary<string, Modifier> attributes)
        {
            return new(
                UnitTrait.GetOrDefault(attributes, prefix, Modifier.None), 
                UnitTrait.GetOrDefault(attributes, prefix + "/cap", Modifier.None));
        }
    }
}
