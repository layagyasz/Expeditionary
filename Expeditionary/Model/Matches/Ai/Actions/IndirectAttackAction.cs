using Expeditionary.Model.Matches.Assets;
using Expeditionary.Model.Matches.Combat;
using Expeditionary.Model.Matches.Orders;
using Expeditionary.Model.Units;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Matches.Ai.Actions
{
    public record class IndirectAttackAction(Vector3i Target, UnitWeaponUsage Attack, UnitWeapon.Mode Mode)
        : IUnitAction
    {
        public bool Do(Match match, MatchUnit unit)
        {
            return match.DoOrder(new IndirectAttackOrder(unit, Attack, Mode, Target));
        }

        public static IEnumerable<IndirectAttackAction> GenerateValidAttacks(Match match, MatchUnit unit)
        {
            var map = match.GetMap();
            foreach (var attack in unit.Type.Weapons)
            {
                foreach (var mode in attack.Weapon!.Modes)
                {
                    if (mode.IsIndirect())
                    {
                        foreach (var target in CombatCalculator.GetValidAttackHexes(mode, map, unit.Position))
                        {
                            yield return new IndirectAttackAction(target.Target, attack, mode);
                        }
                    }
                }
            }
        }
    }
}
