using Expeditionary.Model.Matches.Assets;
using Expeditionary.Model.Matches;
using Expeditionary.Model.Matches.Orders;

namespace Expeditionary.Ai.Actions
{
    public class UnloadAction : IUnitAction
    {
        public bool Do(Match match, Unit unit)
        {
            return match.DoOrder(new UnloadOrder(unit));
        }

        public static IEnumerable<IUnitAction> GenerateValidUnloads(Unit unit)
        {
            if (unit.Passenger != null)
            {
                yield return new UnloadAction();
            }
        }
    }
}
