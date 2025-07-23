using Expeditionary.Model.Units;

namespace Expeditionary.Model.Orders
{
    public record class LoadOrder(Unit Unit, IAsset Target) : IOrder
    {
        public bool Validate(Match match)
        {
            if (!OrderChecker.CanLoad(Unit, Target))
            {
                return false;
            }
            return true;
        }

        public void Execute(Match match)
        {
            Unit.ConsumeAction();
            if (Target is Unit targetUnit)
            {
                targetUnit.ConsumeAction();
            }
            match.Load(Unit, Target);
        }
    }
}
