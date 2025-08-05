using Expeditionary.Model;
using Expeditionary.Model.Units;

namespace Expeditionary.Ai.Actions
{
    public record class IdleAction : IUnitAction
    {
        public bool Do(Match match, Unit unit)
        {
            return true;
        }
    }
}
