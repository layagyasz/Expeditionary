using Expeditionary.Model.Matches;
using Expeditionary.Model.Matches.Assets;

namespace Expeditionary.Ai.Actions
{
    public interface IUnitAction
    {
        bool Do(Match match, Unit unit);
    }
}
