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

        public Modifier GetAccuracy(CombatCondition condition)
        {
            return GetModifiers(condition).Select(x => x.Accuracy).Aggregate(Modifier.None, Modifier.Add);
        }

        public Modifier GetLethality(CombatCondition condition)
        {
            return GetModifiers(condition).Select(x => x.Lethality).Aggregate(Modifier.None, Modifier.Add);
        }

        public Modifier GetVolume(CombatCondition condition)
        {
            return GetModifiers(condition).Select(x => x.Volume).Aggregate(Modifier.None, Modifier.Add);
        }

        private IEnumerable<UnitConditionCapabilities> GetModifiers(CombatCondition condition)
        {
            return Enum.GetValues<CombatCondition>().Where(x => condition.HasFlag(x)).Select(x => _byCondition[x]);
        }
    }
}
