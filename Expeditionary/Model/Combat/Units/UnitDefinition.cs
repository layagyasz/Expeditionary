using Cardamom;
using Cardamom.Collections;
using Cardamom.Json.Collections;
using System.Text.Json.Serialization;

namespace Expeditionary.Model.Combat.Units
{
    public class UnitDefinition : IKeyed
    {
        public class AttackDefinition
        {
            public CombatCondition Condition { get; set; }

            [JsonConverter(typeof(ReferenceCollectionJsonConverter))]
            public List<UnitTrait> Traits { get; set; } = new();
        }

        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Symbol { get; set; }

        public List<AttackDefinition> Attacks { get; set; } = new();

        [JsonConverter(typeof(ReferenceCollectionJsonConverter))]
        public List<UnitTrait> Traits { get; set; } = new();

        public EnumSet<UnitTag> GetTags()
        {
            return Enumerable.Concat(Attacks.SelectMany(x => x.Traits), Traits).SelectMany(x => x.Tags).ToEnumSet();
        }

        public UnitType Build()
        {
            var attributes = Combine(Traits);
            return new(
                this,
                Attacks.Select(x => BuildAttack(Combine(x.Traits))),
                BuildDefenseEnvelope(attributes),
                BuildMovement(attributes),
                BuildCapabilities(attributes),
                BuildIntrinsics(attributes));
        }

        private static UnitAttack BuildAttack(IDictionary<string, UnitModifier> attributes)
        {
            return new()
            {
                Volume = GetOrDefault(attributes, "attack.volume", UnitModifier.None),
                Range = GetOrDefault(attributes, "attack.range", UnitModifier.None),
                Accuracy = GetOrDefault(attributes, "attack.accuracy", UnitModifier.None),
                Tracking = GetOrDefault(attributes, "attack.tracking", UnitModifier.None),
                Penetration = GetOrDefault(attributes, "attack.penetration", UnitModifier.None),
                Lethality = GetOrDefault(attributes, "attack.lethality", UnitModifier.None)
            };
        }

        private static UnitCapabilities BuildCapabilities(IDictionary<string, UnitModifier> attributes)
        {
            return new(
                GetMap<CombatCondition, UnitConditionCapabilities>(
                    "capability", x => BuildConditionCapabilities(x, attributes)));
        }

        private static UnitConditionCapabilities BuildConditionCapabilities(
            string prefix, IDictionary<string, UnitModifier> attributes)
        {
            return new(
                GetOrDefault(attributes, prefix + ".volume" , UnitModifier.None),
                GetOrDefault(attributes, prefix + ".accuracy", UnitModifier.None),
                GetOrDefault(attributes, prefix + ".lethality", UnitModifier.None),
                GetMap<UnitDetectionBand, UnitModifier>(
                    prefix + ".detection", x => GetOrDefault(attributes, x, UnitModifier.None)),
                GetMap<UnitDetectionBand, UnitBoundedValue>(
                    prefix + ".concealment", x => BuildBounded(attributes, x)));
        }

        private static UnitDefense BuildDefenseEnvelope(IDictionary<string, UnitModifier> attributes)
        {
            return new()
            {
                Maneuver = BuildBounded(attributes, "defense.maneuver"),
                Armor = BuildBounded(attributes, "defense.armor"),
                Vitality = BuildBounded(attributes, "defense.vitality")
            };
        }

        private static UnitBoundedValue BuildBounded(IDictionary<string, UnitModifier> attributes, string attribute)
        {
            return new()
            {
                Minimum = GetOrDefault(attributes, attribute + "/min", UnitModifier.None),
                Value = GetOrDefault(attributes, attribute, UnitModifier.None)
            };
        }

        private static UnitMovement.Hindrance BuildHindrance(
            IDictionary<string, UnitModifier> attributes, string attribute)
        {
            return new()
            {
                Minimum = GetOrDefault(attributes, attribute + "/min", UnitModifier.None),
                Maximum = GetOrDefault(attributes, attribute + "/max", UnitModifier.None),
                Cap = GetOrDefault(attributes, attribute + "/cap", UnitModifier.None)
            };
        }

        private static UnitIntrinsics BuildIntrinsics(IDictionary<string, UnitModifier> attributes)
        {
            return new()
            {
                Number = GetOrDefault(attributes, "intrinsic.number", UnitModifier.None),
                Mass = GetOrDefault(attributes, "intrinsic.mass", UnitModifier.None),
                Morale = GetOrDefault(attributes, "intrinsic.morale", UnitModifier.None),
                Power = GetOrDefault(attributes, "intrinsic.power", UnitModifier.None),
                Profile = GetOrDefault(attributes, "intrinsic.profile", UnitModifier.None),
                Stamina = GetOrDefault(attributes, "intrinsic.stamina", UnitModifier.None)
            };
        }

        private static UnitMovement BuildMovement(IDictionary<string, UnitModifier> attributes)
        {
            return new()
            {
                Roughness = BuildHindrance(attributes, "movement.roughness"),
                Softness = BuildHindrance(attributes, "movement.softness"),
                WaterDepth = BuildHindrance(attributes, "movement.waterdepth")
            };
        }

        private static Dictionary<string, UnitModifier> Combine(IEnumerable<UnitTrait> traits)
        {
            return traits.SelectMany(x => x.Modifiers)
                .GroupBy(x => x.Key, x => x.Value)
                .ToDictionary(x => x.Key, x => x.Aggregate((x, y) => x + y));
        }

        private static EnumMap<TKey, TValue> GetMap<TKey, TValue>(
            string prefix, Func<string, TValue> valueGetter)
            where TKey : Enum
        {
            var map = new EnumMap<TKey, TValue>();
            foreach (var key in Enum.GetValues(typeof(TKey)))
            {
                var k = key.ToString()!.ToLower();
                var p = k == "none" ? "" : "[" + k + "]";
                map[(TKey)key] = valueGetter(prefix + p);
            }
            return map;
        }

        private static TValue GetOrDefault<TKey, TValue>(IDictionary<TKey, TValue> dict, TKey key, TValue defaultValue)
        {
            if (dict.TryGetValue(key, out TValue? value))
            {
                return value;
            }
            return defaultValue;
        }
    }
}
