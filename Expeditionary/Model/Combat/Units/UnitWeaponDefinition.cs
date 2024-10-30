using Cardamom;
using Cardamom.Json.Collections;
using System.Text.Json.Serialization;

namespace Expeditionary.Model.Combat.Units
{
    public record class UnitWeaponDefinition : IKeyed
    {
        public record class Mode
        {
            public CombatCondition Condition { get; set; }

            [JsonConverter(typeof(ReferenceCollectionJsonConverter))]
            public List<UnitTrait> Traits { get; set; } = new();
        }

        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        [JsonConverter(typeof(ReferenceCollectionJsonConverter))]
        public List<UnitTrait> Traits { get; set; } = new();
        public List<Mode> Modes { get; set; } = new();

        public UnitWeapon Build()
        {
            return new(Key, Name, this, Modes.Select(x => BuildMode(x, Traits)));
        }

        private static UnitWeapon.Mode BuildMode(Mode definition, List<UnitTrait> baseTraits)
        {
            var attributes = UnitTrait.Combine(Enumerable.Concat(definition.Traits, baseTraits));
            return new()
            {
                Condition = definition.Condition,
                Volume = UnitTrait.GetOrDefault(attributes, "volume", Modifier.None),
                Range = UnitTrait.GetOrDefault(attributes, "range", Modifier.None),
                Accuracy = UnitTrait.GetOrDefault(attributes, "accuracy", Modifier.None),
                Tracking = UnitTrait.GetOrDefault(attributes, "tracking", Modifier.None),
                Penetration = UnitTrait.GetOrDefault(attributes, "penetration", Modifier.None),
                Lethality = UnitTrait.GetOrDefault(attributes, "lethality", Modifier.None),
            };
        }
    }
}
