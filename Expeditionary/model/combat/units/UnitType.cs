using Cardamom.Collections;
using Cardamom.Json;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Expeditionary.model.combat.units
{
    [JsonConverter(typeof(BuilderJsonConverter))]
    [BuilderClass(typeof(UnitDefinition))]
    public class UnitType
    {
        public ImmutableList<UnitAttack> Attack { get; }
        public UnitDefense Defense { get; }
        public UnitPersistence Persistence { get; }
        public UnitSpeed Speed { get; }
        public UnitCapabilities Capabilities { get; }

        public UnitType(
            IEnumerable<UnitAttack> attack,
            UnitDefense defense,
            UnitPersistence persistence,
            UnitSpeed speed,
            UnitCapabilities capabilities)
        {
            Attack = ImmutableList.CreateRange(attack);
            Defense = defense;
            Persistence = persistence;
            Speed = speed;
            Capabilities = capabilities;
        }

        public EnumSet<UnitTag> GetTags()
        {
            return new();
        }
    }
}
