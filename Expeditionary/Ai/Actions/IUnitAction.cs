using Expeditionary.Model;
using Expeditionary.Model.Units;

namespace Expeditionary.Ai.Actions
{
    public interface IUnitAction
    {
        bool Do(Match match, Unit unit);
    }
}
