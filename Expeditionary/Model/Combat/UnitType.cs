using Cardamom.Collections;

namespace Expeditionary.Model.Combat
{
    public class UnitType
    {
        public UnitOffense Offense { get; }
        public UnitDefenseEnvelope Defense { get; }
        public UnitSpeed Speed { get; }
        public UnitCapabilities Capabilities { get; }

        public UnitType(
            UnitOffense offense, UnitDefenseEnvelope defense, UnitSpeed speed, UnitCapabilities capabilities)
        {
            Offense = offense;
            Defense = defense;
            Speed = speed;
            Capabilities = capabilities;
        }

        public EnumSet<UnitTag> GetTags()
        {
            return new();
        }
    }
}
