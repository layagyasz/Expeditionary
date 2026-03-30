using Expeditionary.Model.Matches.Assets;

namespace Expeditionary.Model.Matches.Orders
{
    public record class UnloadOrder(MatchUnit Unit) : IOrder
    {
        public bool Validate(Match match)
        {
            if (Unit.Passenger == null)
            {
                return false;
            }
            if (Unit.Passenger is MatchUnit passenger)
            {
                return Unit.Actions > 0 && passenger.Actions > 0;
            }
            // TODO: Handle other asset classes.
            return false;
        }

        public void Execute(Match match)
        {
            Unit.ConsumeAction();
            ((MatchUnit) Unit.Passenger!).ConsumeAction();
            match.Unload(Unit);
        }
    }
}
