using Expeditionary.Model.Units;

namespace Expeditionary.Model.Orders
{
    public record class UnloadOrder(Unit Unit) : IOrder
    {
        public bool Validate(Match match)
        {
            if (Unit.Passenger == null)
            {
                return false;
            }
            if (Unit.Passenger is Unit passenger)
            {
                return Unit.Actions > 0 && passenger.Actions > 0;
            }
            // TODO: Handle other asset classes.
            return false;
        }

        public void Execute(Match match)
        {
            Unit.ConsumeAction();
            ((Unit) Unit.Passenger!).ConsumeAction();
            match.Unload(Unit);
        }
    }
}
