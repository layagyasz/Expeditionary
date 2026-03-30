using Expeditionary.Model.Matches.Assets;
using Expeditionary.Model.Matches.Orders;

namespace Expeditionary.Model.Matches.Ai.Actions
{
    public record class LoadAction(IMatchAsset Passenger) : IUnitAction
    {
        public bool Do(Match match, MatchUnit unit)
        {
            return match.DoOrder(new LoadOrder(unit, Passenger));
        }

        public static IEnumerable<IUnitAction> GenerateValidLoads(Match match, MatchUnit unit)
        {
            return match.GetAssetsAt(unit.Position)
                .Where(x => OrderChecker.CanLoad(unit, x))
                .Select(x => new LoadAction(x));
        }
    }
}
