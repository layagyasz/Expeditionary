using Expeditionary.Model.Matches.Assets;
using Expeditionary.Model.Matches.Combat;
using Expeditionary.Model.Units;

namespace Expeditionary.Model.Matches.Orders
{
    public record class DirectAttackOrder(MatchUnit Unit, UnitWeaponUsage Weapon, UnitWeapon.Mode Mode, MatchUnit Target) 
        : IOrder
    {
        public bool Validate(Match match)
        {
            if (Unit.Actions == 0)
            {
                return false;
            }
            if (!CombatCalculator.IsValidTarget(Unit, Mode, Target, match.GetMap()))
            {
                return false;
            }
            return true;
        }

        public void Execute(Match match)
        {
            Unit.ConsumeAction();
            var preview = CombatCalculator.GetDirectPreview(Unit, Weapon, Mode, Target, match.GetMap());
            int kills = CombatCalculator.RollKills(preview, match.GetRandom());
            match.Damage(Unit, Target, kills);
        }
    }
}
