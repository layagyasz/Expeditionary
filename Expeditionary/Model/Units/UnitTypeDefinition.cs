using Cardamom;
using Cardamom.Collections;
using Cardamom.Json.Collections;
using Expeditionary.Model.Combat;
using System.Text.Json.Serialization;

namespace Expeditionary.Model.Units
{
    public record class UnitTypeDefinition : IKeyed
    {
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Symbol { get; set; }

        public List<UnitWeaponUsage> Weapons { get; set; } = new();

        [JsonConverter(typeof(ReferenceCollectionJsonConverter))]
        public List<UnitTrait> Traits { get; set; } = new();
        public EnumSet<UnitTag> Tags { get; set; } = new();

        public EnumSet<UnitTag> GetTags()
        {
            if (Tags.Count == 0)
            {
                return Enumerable.Concat(
                    Weapons.SelectMany(x => x.Weapon!.GetTags()), Traits.SelectMany(x => x.Tags)).ToEnumSet();
            }
            return Tags;
        }

        public UnitType Build()
        {
            var attributes = UnitTrait.Combine(Traits);
            return new(
                this,
                Weapons,
                BuildDefenseEnvelope(attributes),
                BuildMovement(attributes),
                BuildCapabilities(attributes),
                BuildIntrinsics(attributes, Weapons));
        }

        private static UnitCapabilities BuildCapabilities(IDictionary<string, Modifier> attributes)
        {
            return new(
                UnitTrait.GetMap<CombatCondition, UnitConditionCapabilities>(
                    "capability", x => BuildConditionCapabilities(x, attributes)));
        }

        private static UnitConditionCapabilities BuildConditionCapabilities(
            string prefix, IDictionary<string, Modifier> attributes)
        {
            return new(
                UnitTrait.GetOrDefault(attributes, prefix + ".volume" , Modifier.None),
                UnitTrait.GetOrDefault(attributes, prefix + ".accuracy", Modifier.None),
                UnitTrait.GetOrDefault(attributes, prefix + ".lethality", Modifier.None),
                UnitTrait.GetMap<UnitDetectionBand, Modifier>(
                    prefix + ".detection", x => UnitTrait.GetOrDefault(attributes, x, Modifier.None)),
                UnitTrait.GetMap<UnitDetectionBand, Modifier>(
                    prefix + ".range", x => UnitTrait.GetOrDefault(attributes, x, Modifier.None)),
                UnitTrait.GetMap<UnitDetectionBand, Modifier>(
                    prefix + ".concealment", x => UnitTrait.GetOrDefault(attributes, x, Modifier.None)),
                UnitTrait.GetMap<UnitDetectionBand, Modifier>(
                    prefix + ".signature", x => UnitTrait.GetOrDefault(attributes, x, Modifier.None)));
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
            return new(
                UnitTrait.GetOrDefault(attributes, attribute + "/min", Modifier.None),
                UnitTrait.GetOrDefault(attributes, attribute, Modifier.None));
        }

        private static Movement.CostFunction BuildHindrance(
            IDictionary<string, Modifier> attributes, string attribute)
        {
            return new()
            {
                Minimum = UnitTrait.GetOrDefault(attributes, attribute + "/min", Modifier.None).GetValue(),
                Maximum = UnitTrait.GetOrDefault(attributes, attribute + "/max", Modifier.None).GetValue(),
                Cap = (int) UnitTrait.GetOrDefault(attributes, attribute + "/cap", Modifier.None).GetValue()
            };
        }

        private static UnitIntrinsics BuildIntrinsics(
            IDictionary<string, Modifier> attributes, IEnumerable<UnitWeaponUsage> weapons)
        {
            var space = BuildSpace("intrinsic.space", attributes);
            space.Used += weapons.Sum(x => x.Number * x.Weapon.Size.GetValue());
            return new()
            {
                Manpower = UnitTrait.GetOrDefault(attributes, "intrinsic.manpower", Modifier.None),
                Mass = BuildMass("intrinsic.mass", attributes) + weapons.Sum(x => x.Number * x.Weapon.Mass.GetValue()),
                Morale = UnitTrait.GetOrDefault(attributes, "intrinsic.morale", Modifier.None),
                Number = UnitTrait.GetOrDefault(attributes, "intrinsic.number", Modifier.None),
                Power = UnitTrait.GetOrDefault(attributes, "intrinsic.power", Modifier.None),
                Profile = UnitTrait.GetOrDefault(attributes, "intrinsic.profile", Modifier.None),
                Space = space,
                Stamina = UnitTrait.GetOrDefault(attributes, "intrinsic.stamina", Modifier.None)
            };
        }

        private static float BuildMass(string prefix, IDictionary<string, Modifier> attributes)
        {
            var amounts = new Dictionary<string, Modifier>();
            var densities = new Dictionary<string, Modifier>();
            foreach (var (attribute, value) in attributes)
            {
                if (attribute.StartsWith(prefix))
                {
                    if (attribute.EndsWith("amount"))
                    {
                        
                        Combine(amounts, attribute[(attribute.IndexOf(':')+1)..^7], value);
                    }
                    if (attribute.EndsWith("density"))
                    {
                        Combine(densities, attribute[(attribute.IndexOf(':')+1)..^8], value);
                    }
                }
            }

            float total = 0f;
            foreach (var key in amounts.Keys.Union(densities.Keys))
            {
                total += amounts.GetValueOrDefault(key, Modifier.None).GetValue() 
                    * densities.GetValueOrDefault(key, Modifier.None).GetValue();
            }
            return total;
        }

        private static Movement BuildMovement(IDictionary<string, Modifier> attributes)
        {
            return new()
            {
                Restriction = BuildHindrance(attributes, "movement.restriction"),
                Roughness = BuildHindrance(attributes, "movement.roughness"),
                Softness = BuildHindrance(attributes, "movement.softness"),
                Slope = BuildHindrance(attributes, "movement.slope"),
                WaterDepth = BuildHindrance(attributes, "movement.waterdepth")
            };
        }

        private static UnitSpace BuildSpace(string prefix, IDictionary<string, Modifier> attributes)
        {
            return new()
            {
                Available = UnitTrait.GetOrDefault(attributes, prefix, Modifier.None).GetValue(),
                Used = UnitTrait.GetOrDefault(attributes, prefix + "/used", Modifier.None).GetValue()
            };
        }

        private static void Combine(IDictionary<string, Modifier> attributes, string key, Modifier modifier)
        {
            if (attributes.TryGetValue(key, out var value)) 
            {
                attributes[key] = value + modifier;
            }
            else
            {
                attributes.Add(key, modifier);
            }
        }
    }
}
