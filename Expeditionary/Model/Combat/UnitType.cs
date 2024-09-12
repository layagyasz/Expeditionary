using Cardamom.Collections;
using System.Collections.Immutable;

namespace Expeditionary.Model.Combat
{
    public class UnitType
    {
        public ImmutableList<UnitOffense> Offense { get; }
        public UnitDefenseEnvelope Defense { get; }
        public UnitPersistence Persistence { get; }
        public UnitSpeed Speed { get; }
        public UnitCapabilities Capabilities { get; }

        public UnitType(
            IEnumerable<UnitOffense> offense, 
            UnitDefenseEnvelope defense, 
            UnitPersistence persistence, 
            UnitSpeed speed,
            UnitCapabilities capabilities)
        {
            Offense = ImmutableList.CreateRange(offense);
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
