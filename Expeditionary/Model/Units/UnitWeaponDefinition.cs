using Cardamom;
using Cardamom.Json;
using Cardamom.Json.Collections;
using Expeditionary.Model.Matches.Combat;
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
            var baseAttributes = UnitTrait.Combine(extraTraits.Concat(Traits));
            return new(
                Key,
                Name,
                GetTags(),
                Modes.Select(x => BuildMode(x, extraTraits.Concat(Traits))),
                UnitTrait.GetOrDefault(baseAttributes, "mass", Modifier.None),
                UnitTrait.GetOrDefault(baseAttributes, "size", Modifier.None),
                GetPoints());
        }

        public int GetPoints()
        {
            return Traits.Sum(x => x.Cost);
        }

        public IEnumerable<UnitTag> GetTags()
        {
            return Traits.Concat(Modes.SelectMany(x => x.Traits)).SelectMany(x => x.Tags);
        }

        private static UnitWeapon.Mode BuildMode(Mode definition, IEnumerable<UnitTrait> baseTraits)
        {
            var attributes = UnitTrait.Combine(definition.Traits.Concat(baseTraits));
            return new()
            {
                Condition = definition.Condition,
                Volume = UnitTrait.GetOrDefault(attributes, "volume", Modifier.None),
                Range = BuildRange("range", attributes),
                Accuracy = UnitTrait.GetOrDefault(attributes, "accuracy", Modifier.None),
                Radius = UnitTrait.GetOrDefault(attributes, "radius", Modifier.None),
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
                UnitTrait.GetOrDefault(attributes, prefix + "/cap", Modifier.None),
                UnitTrait.GetOrDefault(attributes, prefix + "/min", Modifier.None));
        }
    }
}
