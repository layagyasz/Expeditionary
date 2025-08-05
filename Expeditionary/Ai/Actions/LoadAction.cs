using Expeditionary.Model;
using Expeditionary.Model.Orders;
using Expeditionary.Model.Units;

namespace Expeditionary.Ai.Actions
{
    public record class LoadAction(IAsset Passenger) : IUnitAction
    {
        public bool Do(Match match, Unit unit)
        {
            return match.DoOrder(new LoadOrder(unit, Passenger));
        }

        public static IEnumerable<IUnitAction> GenerateValidLoads(Match match, Unit unit)
        {
            return match.GetAssetsAt(unit.Position!.Value)
                .Where(x => OrderChecker.CanLoad(unit, x))
                .Select(x => new LoadAction(x));
        }
    }
}
