﻿using Cardamom;
using Cardamom.Collections;
using Cardamom.Json.Collections;
using System.Text.Json.Serialization;

namespace Expeditionary.Model.Combat.Units
{
    public record class UnitTypeDefinition : IKeyed
    {
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Symbol { get; set; }

        public List<UnitWeaponUsage> Weapons { get; set; } = new();

        [JsonConverter(typeof(ReferenceCollectionJsonConverter))]
        public List<UnitTrait> Traits { get; set; } = new();

        public EnumSet<UnitTag> GetTags()
        {
            return Enumerable.Concat(
                Weapons.SelectMany(x => x.Weapon!.GetTags()), Traits.SelectMany(x => x.Tags)).ToEnumSet();
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
                BuildIntrinsics(attributes));
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
            return new()
            {
                Minimum = UnitTrait.GetOrDefault(attributes, attribute + "/min", Modifier.None),
                Value = UnitTrait.GetOrDefault(attributes, attribute, Modifier.None)
            };
        }

        private static Movement.CostFunction BuildHindrance(
            IDictionary<string, Modifier> attributes, string attribute)
        {
            return new()
            {
                Minimum = UnitTrait.GetOrDefault(attributes, attribute + "/min", Modifier.None),
                Maximum = UnitTrait.GetOrDefault(attributes, attribute + "/max", Modifier.None),
                Cap = UnitTrait.GetOrDefault(attributes, attribute + "/cap", Modifier.None)
            };
        }

        private static UnitIntrinsics BuildIntrinsics(IDictionary<string, Modifier> attributes)
        {
            return new()
            {
                Manpower = UnitTrait.GetOrDefault(attributes, "intrinsic.manpower", Modifier.None),
                Mass = BuildMass("intrinsic.mass", attributes),
                Morale = UnitTrait.GetOrDefault(attributes, "intrinsic.morale", Modifier.None),
                Number = UnitTrait.GetOrDefault(attributes, "intrinsic.number", Modifier.None),
                Power = UnitTrait.GetOrDefault(attributes, "intrinsic.power", Modifier.None),
                Profile = UnitTrait.GetOrDefault(attributes, "intrinsic.profile", Modifier.None),
                Space = BuildSpace("intrinsic.space", attributes),
                Stamina = UnitTrait.GetOrDefault(attributes, "intrinsic.stamina", Modifier.None)
            };
        }

        private static UnitMass BuildMass(string prefix, IDictionary<string, Modifier> attributes)
        {
            return new()
            {
                Armor = BuildMassComponent(prefix + ".armor", attributes),
                Body = BuildMassComponent(prefix + ".body", attributes),
                Equipment = BuildMassComponent(prefix + ".equipment", attributes)
            };
        }

        private static UnitMassComponent BuildMassComponent(string prefix, IDictionary<string, Modifier> attributes)
        {
            return new()
            {
                Density = UnitTrait.GetOrDefault(attributes, prefix + "/density", Modifier.None),
                Amount = UnitTrait.GetOrDefault(attributes, prefix + "/amount", Modifier.None)
            };
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
                Available = UnitTrait.GetOrDefault(attributes, prefix, Modifier.None),
                Used = UnitTrait.GetOrDefault(attributes, prefix + "/used", Modifier.None)
            };
        }
    }
}
