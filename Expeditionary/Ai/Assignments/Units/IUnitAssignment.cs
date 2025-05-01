using Expeditionary.Model;

namespace Expeditionary.Ai.Assignments.Units
{
    public interface IUnitAssignment
    {
        void Place(UnitHandler unit, Match match);
    }
}
