using Expeditionary.Model.Matches.Assets;

namespace Expeditionary.Model.Matches.Ai.Actions
{
    public record class IdleAction : IUnitAction
    {
        public bool Do(Match match, MatchUnit unit)
        {
            return true;
        }
    }
}
