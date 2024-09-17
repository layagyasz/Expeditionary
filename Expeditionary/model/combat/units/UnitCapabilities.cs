using Cardamom.Collections;

namespace Expeditionary.model.combat.units
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
