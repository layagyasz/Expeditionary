using Expeditionary.Model.Matches.Assets;

namespace Expeditionary.Model.Matches.Orders
{
    public record class LoadOrder(MatchUnit Unit, IMatchAsset Target) : IOrder
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
            if (Target is MatchUnit targetUnit)
            {
                targetUnit.ConsumeAction();
            }
            match.Load(Unit, Target);
        }
    }
}
