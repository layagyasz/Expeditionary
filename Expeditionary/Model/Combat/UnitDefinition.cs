namespace Expeditionary.Model.Combat
{
    public class UnitDefinition
    {
        public List<List<UnitTraitSet>> Offenses { get; set; } = new();
        public List<UnitTraitSet> Traits { get; set; } = new();

        public UnitType Build()
        {
            var attributes = Combine(Traits);
            return new(
                Offenses.Select(x => BuildOffense(Combine(x))),
                BuildDefenseEnvelope(attributes), 
                BuildPersistence(attributes), 
                BuildSpeed(attributes),
                BuildCapabilities(attributes));
        }

        private static UnitCapabilities BuildCapabilities(IDictionary<string, float> attributes)
        {
            return new()
            {
                Detection = GetOrDefault(attributes, "capability.detection", 0),
                Concealment = GetOrDefault(attributes, "capability.concealment", 0)
            };
        }

        private static UnitDefenseEnvelope BuildDefenseEnvelope(IDictionary<string, float> attributes)
        {
            return new()
            {
                Profile = BuildDefense(attributes, "defense.profile"),
                Maneuver = BuildDefense(attributes, "defense.maneuver"),
                Armor = BuildDefense(attributes, "defense.armor"),
                Vitality = BuildDefense(attributes, "defense.vitality")
            };
        }

        private static UnitDefense BuildDefense(IDictionary<string, float> attributes, string attribute)
        {
            return new()
            {
                Minimum = GetOrDefault(attributes, attribute + ".min", 0),
                Value = GetOrDefault(attributes, attribute, 0)
            };
        }

        private static UnitOffense BuildOffense(IDictionary<string, float> attributes)
        {
            return new()
            {
                Volume = GetOrDefault(attributes, "attack.volume", 0),
                Range = GetOrDefault(attributes, "attack.range", 0),
                Accuracy = GetOrDefault(attributes, "attack.accuracy", 0),
                Tracking = GetOrDefault(attributes, "attack.tracking", 0),
                Penetration = GetOrDefault(attributes, "attack.penetration", 0),
                Lethality = GetOrDefault(attributes, "attack.lethality", 0)
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

        private static Dictionary<string, float> Combine(IEnumerable<UnitTraitSet> traitSets)
        {
            return traitSets.SelectMany(x => x.Traits)
                .SelectMany(x => x.Modifiers)
                .GroupBy(x => x.Key, x => x.Value)
                .ToDictionary(x => x.Key, x => x.Aggregate((x,y) => x + y).GetValue());
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
