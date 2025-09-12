using Expeditionary.Model.Combat;
using Expeditionary.Model.Units;
using MathNet.Numerics.Distributions;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Orders
{
    public record class IndirectAttackOrder(Unit Unit, UnitWeaponUsage Weapon, UnitWeapon.Mode Mode, Vector3i Target)
        : IOrder
    {
        public bool Validate(Match match)
        {
            if (Unit.Actions == 0)
            {
                return false;
            }
            if (!CombatCalculator.IsValidLineOfSight(Mode, match.GetMap(), Unit.Position!.Value, Target))
            {
                return false;
            }
            return true;
        }

        public void Execute(Match match)
        {
            Unit.ConsumeAction();
            foreach (var targetAsset in match.GetAssetsAt(Target))
            {
                if (targetAsset is Unit targetUnit)
                {
                    var preview = CombatCalculator.GetDirectPreview(Unit, Weapon, Mode, targetUnit, match.GetMap());
                    int kills = (int)preview.Result + Bernoulli.Sample(match.GetRandom(), preview.Result % 1);
                    match.Damage(Unit, targetUnit, kills);
                }
            }
        }
    }
}
