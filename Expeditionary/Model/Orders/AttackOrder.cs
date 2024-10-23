using Expeditionary.Hexagons;
using Expeditionary.Model.Combat;
using Expeditionary.Model.Combat.Units;
using MathNet.Numerics.Distributions;

namespace Expeditionary.Model.Orders
{
    public class AttackOrder : IOrder
    {
        public Unit Unit { get; }
        public UnitAttack Attack { get; }
        public Unit Defender { get; }

        public AttackOrder(Unit attacker, UnitAttack attack, Unit defender)
        {
            Unit = attacker;
            Attack = attack;
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
            if (Geometry.GetCubicDistance(Unit.Position, Defender.Position) > Attack.Range.GetValue())
            {
                return false;
            }
            if (!Sighting.IsValidLineOfSight(match.GetMap(), Unit.Position, Defender.Position))
            {
                return false;
            }
            return true;
        }

        public void Execute(Match match, Random random)
        {
            Unit.Attacked = true;
            var preview = CombatCalculator.GetPreview(Unit, Attack, Defender, match.GetMap());
            int kills = (int)preview.Result + Bernoulli.Sample(random, preview.Result % 1);
            Defender.Damage(kills, random);
            Console.WriteLine(preview);
            Console.WriteLine(kills);
        }
    }
}
