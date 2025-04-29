using Expeditionary.Model;

namespace Expeditionary.Ai.Assignments.Units
{
    public interface IUnitAssignment
    {
        void Place(UnitAssignment unit, Match match);
    }
}
