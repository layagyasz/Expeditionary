using Expeditionary.Model;
using Expeditionary.Model.Units;

namespace Expeditionary.Ai.Assignments.Units
{
    public interface IUnitAction
    {
        void Do(Match match, Unit unit);
        float GetValue(Match match, Unit unit);
    }
}
