using Expeditionary.Model.Units;

namespace Expeditionary.Model.Orders
{
    public class MoveOrder : IOrder
    {
        public Unit Unit { get; }
        public Pathing.Path? Path { get; }

        public MoveOrder(Unit unit, Pathing.Path? path)
        {
            Unit = unit;
            Path = path;
        }

        public bool Validate(Match match)
        {
            return Path != null && Path.Cost <= Unit.Movement;
        }

        public void Execute(Match match, Random random)
        {
            Unit.Movement -= Path!.Cost;
            match.Move(Unit, Path!);
        }
    }
}
