using Expeditionary.Model;
using Expeditionary.Model.Combat;
using Expeditionary.Model.Knowledge;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Orders;
using Expeditionary.Model.Units;

namespace Expeditionary.Ai.Actions
{
    public record class AttackAction(Unit Target, UnitWeaponUsage Attack, UnitWeapon.Mode Mode) : IUnitAction
    {
        public bool Do(Match match, Unit unit)
        {
            return match.DoOrder(new AttackOrder(unit, Attack, Mode, Target));
        }

        public static IEnumerable<AttackAction> GenerateValidAttacks(
            Match match, IPlayerKnowledge knowledge, Unit unit)
        {
            var map = match.GetMap();
            foreach (var attack in unit.Type.Weapons)
            {
                foreach (var mode in attack.Weapon!.Modes)
                {
                    foreach (var target in FindValidTargets(match, knowledge, unit, mode, map))
                    {
                        yield return new AttackAction(target, attack, mode);
                    }
                }
            }
        }

        private static IEnumerable<Unit> FindValidTargets(
            Match match, IPlayerKnowledge knowledge, Unit unit, UnitWeapon.Mode mode, Map map)
        {
            return match.GetAssets()
                .Where(x => !x.IsDestroyed)
                .Where(x => knowledge.GetAsset(x).IsVisible)
                .Where(x => x is Unit)
                .Cast<Unit>()
                .Where(x => CombatCalculator.IsValidTarget(unit, mode, x, map));
        }
    }
}
