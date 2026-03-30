using Expeditionary.Model.Matches.Assets;

namespace Expeditionary.Model.Matches.Ai.Actions
{
    public interface IUnitAction
    {
        bool Do(Match match, MatchUnit unit);
    }
}
