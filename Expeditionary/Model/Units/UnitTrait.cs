using Cardamom;
using Cardamom.Collections;

namespace Expeditionary.Model.Units
{
    public record class UnitTrait : IKeyed
    {
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Family { get; set; } = string.Empty;
        public int Cost { get; set; }
        public int Level { get; set; }
        public EnumSet<UnitTag> Tags { get; set; } = new();
        public Dictionary<string, Modifier> Modifiers { get; set; } = new();

        public static Dictionary<string, Modifier> Combine(IEnumerable<UnitTrait> traits)
        {
            return traits.SelectMany(x => x.Modifiers)
                .GroupBy(x => x.Key, x => x.Value)
                .ToDictionary(x => x.Key, x => x.Aggregate(Modifier.None, (x, y) => x + y));
        }

        public static EnumMap<TKey, TValue> GetMap<TKey, TValue>(
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

        public static TValue GetOrDefault<TKey, TValue>(IDictionary<TKey, TValue> dict, TKey key, TValue defaultValue)
        {
            if (dict.TryGetValue(key, out TValue? value))
            {
                return value;
            }
            return defaultValue;
        }
    }
}
