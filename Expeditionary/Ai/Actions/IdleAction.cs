using Expeditionary.Model;
using Expeditionary.Model.Units;

namespace Expeditionary.Ai.Actions
{
    public record class IdleAction : IUnitAction
    {
        public void Do(Match match, Unit unit) { }
    }
}
