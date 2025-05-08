using Expeditionary.Ai.Assignments.Formations;
using Expeditionary.Ai.Assignments.Units;

namespace Expeditionary.Ai.Assignments
{
    public record class FormationAssignment(
        Dictionary<SimpleFormationHandler, IFormationAssignment> ChildFormationAssignments, 
        Dictionary<UnitHandler, IUnitAssignment> UnitAssignments);
}
