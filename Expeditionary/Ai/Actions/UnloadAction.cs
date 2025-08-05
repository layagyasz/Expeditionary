using Expeditionary.Model.Orders;
using Expeditionary.Model.Units;
using Expeditionary.Model;

namespace Expeditionary.Ai.Actions
{
    public class UnloadAction : IUnitAction
    {
        public bool Do(Match match, Unit unit)
        {
            return match.DoOrder(new UnloadOrder(unit));
        }

        public static IEnumerable<IUnitAction> GenerateValidLoads(Match match, Unit unit)
        {
            if (unit.Passenger != null)
            {
                yield return new UnloadAction();
            }
        }
    }
}
