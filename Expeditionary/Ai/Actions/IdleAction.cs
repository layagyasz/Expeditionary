using Expeditionary.Model.Matches;
using Expeditionary.Model.Matches.Assets;

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
