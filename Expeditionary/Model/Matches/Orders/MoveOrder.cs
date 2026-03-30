using Expeditionary.Model.Matches.Assets;

namespace Expeditionary.Model.Matches.Orders
{
    public record class MoveOrder(MatchUnit Unit, Pathing.Path? Path) : IOrder
    {
        public bool Validate(Match match)
        {
            return Path != null && Unit.Actions > 0 && Unit.Type.Speed >= Path.Cost && !Unit.IsPassenger;
        }

        public void Execute(Match match)
        {
            Unit.ConsumeAction();
            match.Move(Unit, Path!);
            if (Unit.Passenger != null)
            {
                match.Move(Unit, Path!);
            }
        }
    }
}
