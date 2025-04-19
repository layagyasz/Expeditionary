using Expeditionary.Hexagons;
using Expeditionary.Model.Combat;
using Expeditionary.Model.Units;
using MathNet.Numerics.Distributions;

namespace Expeditionary.Model.Orders
{
    public class AttackOrder : IOrder
    {
        public Unit Unit { get; }
        public UnitWeaponUsage Weapon { get; }
        public UnitWeapon.Mode Mode { get; }
        public Unit Defender { get; }

        public AttackOrder(Unit attacker, UnitWeaponUsage weapon, UnitWeapon.Mode mode, Unit defender)
        {
            Unit = attacker;
            Weapon = weapon;
            Mode = mode;
            Defender = defender;
        }

        public bool Validate(Match match)
        {
            if (Unit.Attacked)
            {
                return false;
            }
            if (Unit.Player.Team == Defender.Player.Team)
            {
                return false;
            }
            if (Geometry.GetCubicDistance(Unit.Position!.Value, Defender.Position!.Value) > Mode.Range.Get())
            {
                return false;
            }
            if (!Sighting.IsValidLineOfSight(match.GetMap(), Unit.Position!.Value, Defender.Position!.Value))
            {
                return false;
            }
            return true;
        }

        public void Execute(Match match)
        {
            Unit.Attacked = true;
            var preview = CombatCalculator.GetPreview(Unit, Weapon, Mode, Defender, match.GetMap());
            int kills = (int)preview.Result + Bernoulli.Sample(match.GetRandom(), preview.Result % 1);
            match.Damage(Unit, Defender, kills);
            Console.WriteLine(preview);
            Console.WriteLine(kills);
        }
    }
}
