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

        private static UnitAttack BuildAttack(IDictionary<string, Modifier> attributes)
        {
            return new()
            {
                Volume = GetOrDefault(attributes, "attack.volume", Modifier.None),
                Range = GetOrDefault(attributes, "attack.range", Modifier.None),
                Accuracy = GetOrDefault(attributes, "attack.accuracy", Modifier.None),
                Tracking = GetOrDefault(attributes, "attack.tracking", Modifier.None),
                Penetration = GetOrDefault(attributes, "attack.penetration", Modifier.None),
                Lethality = GetOrDefault(attributes, "attack.lethality", Modifier.None)
            };
        }

        private static UnitCapabilities BuildCapabilities(IDictionary<string, Modifier> attributes)
        {
            return new(
                GetMap<CombatCondition, UnitConditionCapabilities>(
                    "capability", x => BuildConditionCapabilities(x, attributes)));
        }

        private static UnitConditionCapabilities BuildConditionCapabilities(
            string prefix, IDictionary<string, Modifier> attributes)
        {
            return new(
                GetOrDefault(attributes, prefix + ".volume" , Modifier.None),
                GetOrDefault(attributes, prefix + ".accuracy", Modifier.None),
                GetOrDefault(attributes, prefix + ".lethality", Modifier.None),
                GetMap<UnitDetectionBand, Modifier>(
                    prefix + ".detection", x => GetOrDefault(attributes, x, Modifier.None)),
                GetMap<UnitDetectionBand, UnitBoundedValue>(
                    prefix + ".concealment", x => BuildBounded(attributes, x)));
        }

        private static UnitDefense BuildDefenseEnvelope(IDictionary<string, Modifier> attributes)
        {
            return new()
            {
                Maneuver = BuildBounded(attributes, "defense.maneuver"),
                Armor = BuildBounded(attributes, "defense.armor"),
                Vitality = BuildBounded(attributes, "defense.vitality")
            };
        }

        private static UnitBoundedValue BuildBounded(IDictionary<string, Modifier> attributes, string attribute)
        {
            return new()
            {
                Minimum = GetOrDefault(attributes, attribute + "/min", Modifier.None),
                Value = GetOrDefault(attributes, attribute, Modifier.None)
            };
        }

        private static Movement.Hindrance BuildHindrance(
            IDictionary<string, Modifier> attributes, string attribute)
        {
            return new()
            {
                Minimum = GetOrDefault(attributes, attribute + "/min", Modifier.None),
                Maximum = GetOrDefault(attributes, attribute + "/max", Modifier.None),
                Cap = GetOrDefault(attributes, attribute + "/cap", Modifier.None)
            };
        }

        private static UnitIntrinsics BuildIntrinsics(IDictionary<string, Modifier> attributes)
        {
            return new()
            {
                Number = GetOrDefault(attributes, "intrinsic.number", Modifier.None),
                Mass = GetOrDefault(attributes, "intrinsic.mass", Modifier.None),
                Morale = GetOrDefault(attributes, "intrinsic.morale", Modifier.None),
                Power = GetOrDefault(attributes, "intrinsic.power", Modifier.None),
                Profile = GetOrDefault(attributes, "intrinsic.profile", Modifier.None),
                Stamina = GetOrDefault(attributes, "intrinsic.stamina", Modifier.None)
            };
        }

        private static Movement BuildMovement(IDictionary<string, Modifier> attributes)
        {
            return new()
            {
                Roughness = BuildHindrance(attributes, "movement.roughness"),
                Softness = BuildHindrance(attributes, "movement.softness"),
                WaterDepth = BuildHindrance(attributes, "movement.waterdepth")
            };
        }

        private static Dictionary<string, Modifier> Combine(IEnumerable<UnitTrait> traits)
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
