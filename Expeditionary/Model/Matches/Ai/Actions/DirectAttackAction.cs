using Expeditionary.Model.Mapping;
using Expeditionary.Model.Matches.Assets;
using Expeditionary.Model.Matches.Combat;
using Expeditionary.Model.Matches.Knowledge;
using Expeditionary.Model.Matches.Orders;
using Expeditionary.Model.Units;

namespace Expeditionary.Model.Matches.Ai.Actions
{
    public record class DirectAttackAction(MatchUnit Target, UnitWeaponUsage Attack, UnitWeapon.Mode Mode) : IUnitAction
    {
        public bool Do(Match match, MatchUnit unit)
        {
            return match.DoOrder(new DirectAttackOrder(unit, Attack, Mode, Target));
        }

        public static IEnumerable<DirectAttackAction> GenerateValidAttacks(
            Match match, IPlayerKnowledge knowledge, MatchUnit unit)
        {
            var map = match.GetMap();
            foreach (var attack in unit.Type.Weapons)
            {
                foreach (var mode in attack.Weapon!.Modes)
                {
                    if (!mode.IsIndirect())
                    {
                        foreach (var target in FindValidTargets(match, knowledge, unit, mode, map))
                        {
                            yield return new DirectAttackAction(target, attack, mode);
                        }
                    }
                }
            }
        }

        private static IEnumerable<MatchUnit> FindValidTargets(
            Match match, IPlayerKnowledge knowledge, MatchUnit unit, UnitWeapon.Mode mode, Map map)
        {
            return match.GetAssets()
                .Where(x => !x.IsDestroyed)
                .Where(x => knowledge.GetAsset(x).IsVisible)
                .Where(x => x is MatchUnit)
                .Cast<MatchUnit>()
                .Where(x => CombatCalculator.IsValidTarget(unit, mode, x, map));
        }
    }
}
