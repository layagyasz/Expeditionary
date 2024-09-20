using Cardamom;
using Cardamom.Collections;
using Cardamom.Json;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Expeditionary.Model.Combat.Units
{
    [JsonConverter(typeof(BuilderJsonConverter))]
    [BuilderClass(typeof(UnitDefinition))]
    public class UnitType : IKeyed
    {
        public string Key
        {
            get => Definition.Key; 
            set => Definition.Key = value;
        }
        public string Name => Definition.Name;
        public string? Symbol => Definition.Symbol;
        public UnitDefinition Definition { get; }
        public ImmutableList<UnitAttack> Attack { get; }
        public UnitDefense Defense { get; }
        public UnitPersistence Persistence { get; }
        public UnitSpeed Speed { get; }
        public UnitCapabilities Capabilities { get; }

        public UnitType(
            UnitDefinition definition,
            IEnumerable<UnitAttack> attack,
            UnitDefense defense,
            UnitPersistence persistence,
            UnitSpeed speed,
            UnitCapabilities capabilities)
        {
            Definition = definition;
            Attack = ImmutableList.CreateRange(attack);
            Defense = defense;
            Persistence = persistence;
            Speed = speed;
            Capabilities = capabilities;
        }

        public EnumSet<UnitTag> GetTags()
        {
            return Definition.GetTags();
        }
    }
}
