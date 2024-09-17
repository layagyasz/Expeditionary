using Cardamom;
using Cardamom.Collections;
using Cardamom.Json.Collections;
using System.Text.Json.Serialization;

namespace Expeditionary.Model.Combat
{
    public class UnitDefinition : IKeyed
    {
        public class UnitTraitSet
        {
            [JsonConverter(typeof(ReferenceCollectionJsonConverter))]
            public List<UnitTrait> Traits { get; set; } = new();
        }

        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Symbol { get; set; }

        public List<UnitTraitSet> Offenses { get; set; } = new();

        [JsonConverter(typeof(ReferenceCollectionJsonConverter))]
        public List<UnitTrait> Traits { get; set; } = new();

        public UnitType Build()
        {
            var attributes = Combine(Traits);
            return new(
                Offenses.Select(x => BuildOffense(Combine(x.Traits), attributes)),
                BuildDefenseEnvelope(attributes), 
                BuildPersistence(attributes), 
                BuildSpeed(attributes),
                BuildCapabilities(attributes));
        }

        private static UnitCapabilities BuildCapabilities(IDictionary<string, float> attributes)
        {
            return new()
            {
                Detection = 
                    GetMap<UnitDetectionBand, float>("capabilities.detection", x => GetOrDefault(attributes, x, 0)),
                Concealment = 
                    GetMap<UnitDetectionBand, UnitBoundedValue>(
                        "capabilities.concealment", x => BuildBounded(attributes, x))
            };
        }

        private static UnitDefense BuildDefenseEnvelope(IDictionary<string, float> attributes)
        {
            return new()
            {
                Profile = BuildBounded(attributes, "defense.profile"),
                Maneuver = BuildBounded(attributes, "defense.maneuver"),
                Armor = BuildBounded(attributes, "defense.armor"),
                Vitality = BuildBounded(attributes, "defense.vitality")
            };
        }

        private static UnitBoundedValue BuildBounded(IDictionary<string, float> attributes, string attribute)
        {
            return new()
            {
                Minimum = GetOrDefault(attributes, attribute + ".min", 0),
                Value = GetOrDefault(attributes, attribute, 0)
            };
        }

        private static UnitOffense BuildOffense(
            IDictionary<string, float> attributes, IDictionary<string, float> globalAttributes)
        {
            return new()
            {
                Volume = GetOrDefault(attributes, "attack.volume", 0) 
                    + GetOrDefault(globalAttributes, "attack.volume", 0),
                Range = GetOrDefault(attributes, "attack.range", 0)
                    + GetOrDefault(globalAttributes, "attack.range", 0),
                Accuracy = GetOrDefault(attributes, "attack.accuracy", 0)
                    + GetOrDefault(globalAttributes, "attack.accuracy", 0),
                Tracking = GetOrDefault(attributes, "attack.tracking", 0)
                    + GetOrDefault(globalAttributes, "attack.tracking", 0),
                Penetration = GetOrDefault(attributes, "attack.penetration", 0) 
                    + GetOrDefault(globalAttributes, "attack.penetration", 0),
                Lethality = GetOrDefault(attributes, "attack.lethality", 0)
                    + GetOrDefault(globalAttributes, "attack.lethality", 0)
            };
        }

        private static UnitPersistence BuildPersistence(IDictionary<string, float> attributes)
        {
            return new()
            {
                Number = GetOrDefault(attributes, "persistence.number", 0),
                Morale = GetOrDefault(attributes, "persistence.morale", 0),
                Stamina = GetOrDefault(attributes, "persistence.stamina", 0)
            };
        }

        private static UnitSpeed BuildSpeed(IDictionary<string, float> attributes)
        {
            return new();
        }

        private static Dictionary<string, float> Combine(IEnumerable<UnitTrait> traits)
        {
            return traits.SelectMany(x => x.Modifiers)
                .GroupBy(x => x.Key, x => x.Value)
                .ToDictionary(x => x.Key, x => x.Aggregate((x,y) => x + y).GetValue());
        }

        private static EnumMap<TKey, TValue> GetMap<TKey, TValue>(
            string prefix, Func<string, TValue> valueGetter)
            where TKey : Enum
        {
            var map = new EnumMap<TKey, TValue>();
            foreach (var key in Enum.GetValues(typeof(TKey)))
            {
                map[(TKey)key] = valueGetter(prefix + "." + key.ToString()!.ToLower());
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
