using Cardamom.Collections;

namespace Expeditionary.Model.Combat.Units
{
    public class UnitCapabilities
    {
        private readonly EnumMap<CombatCondition, UnitConditionCapabilities> _byCondition;

        public UnitCapabilities(EnumMap<CombatCondition, UnitConditionCapabilities> byCondition)
        {
            _byCondition = byCondition;
        }
    }
}
