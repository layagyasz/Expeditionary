using Expeditionary.Model.Matches.Assets;
using Expeditionary.Model.Matches.Orders;

namespace Expeditionary.Model.Matches.Ai.Actions
{
    public class UnloadAction : IUnitAction
    {
        public bool Do(Match match, MatchUnit unit)
        {
            return match.DoOrder(new UnloadOrder(unit));
        }

        public static IEnumerable<IUnitAction> GenerateValidUnloads(MatchUnit unit)
        {
            if (unit.Passenger != null)
            {
                yield return new UnloadAction();
            }
        }
    }
}
