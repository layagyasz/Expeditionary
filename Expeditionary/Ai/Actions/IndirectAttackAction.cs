using Expeditionary.Model;
using Expeditionary.Model.Combat;
using Expeditionary.Model.Orders;
using Expeditionary.Model.Units;
using OpenTK.Mathematics;

namespace Expeditionary.Ai.Actions
{
    public record class IndirectAttackAction(Vector3i Target, UnitWeaponUsage Attack, UnitWeapon.Mode Mode)
        : IUnitAction
    {
        public bool Do(Match match, Unit unit)
        {
            return match.DoOrder(new IndirectAttackOrder(unit, Attack, Mode, Target));
        }

        public static IEnumerable<IndirectAttackAction> GenerateValidAttacks(Match match, Unit unit)
        {
            var map = match.GetMap();
            foreach (var attack in unit.Type.Weapons)
            {
                foreach (var mode in attack.Weapon!.Modes)
                {
                    if (mode.IsIndirect())
                    {
                        foreach (var target in CombatCalculator.GetValidAttackHexes(mode, map, unit.Position!.Value))
                        {
                            yield return new IndirectAttackAction(target.Target, attack, mode);
                        }
                    }
                }
            }
        }
    }
}
