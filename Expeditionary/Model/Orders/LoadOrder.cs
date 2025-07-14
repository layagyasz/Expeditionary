using Expeditionary.Model.Units;

namespace Expeditionary.Model.Orders
{
    public record class LoadOrder(Unit Unit, IAsset Target) : IOrder
    {
        public bool Validate(Match match)
        {
            // TODO: Check that Unit can carry and Target can be carried.
            if (Target is Unit target)
            {
                return Unit.Actions > 0
                    && target.Actions > 0
                    && Unit.Player == target.Player
                    && Unit.Passenger == null
                    && !Unit.IsPassenger
                    && !Target.IsPassenger;
            }
            // TODO: Implement for other asset classes.
            return false;
        }

        public void Execute(Match match)
        {
            Unit.ConsumeAction();
            ((Unit) Target).ConsumeAction();
            match.Load(Unit, Target);
        }
    }
}
