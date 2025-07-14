using Expeditionary.Model.Combat;
using Expeditionary.Model.Units;
using MathNet.Numerics.Distributions;

namespace Expeditionary.Model.Orders
{
    public record class AttackOrder(Unit Unit, UnitWeaponUsage Weapon, UnitWeapon.Mode Mode, Unit Target) : IOrder
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
            var preview = CombatCalculator.GetPreview(Unit, Weapon, Mode, Target, match.GetMap());
            int kills = (int)preview.Result + Bernoulli.Sample(match.GetRandom(), preview.Result % 1);
            match.Damage(Unit, Target, kills);
        }
    }
}
