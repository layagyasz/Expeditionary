using Expeditionary.Ai.Assignments.Formations;
using Expeditionary.Ai.Assignments.Units;

namespace Expeditionary.Ai.Assignments
{
    public record class FormationAssignment(
        Dictionary<SimpleFormationHandler, IFormationAssignment> ChildFormationAssignments, 
        Dictionary<UnitHandler, IUnitAssignment> UnitAssignments)
    {
        public static FormationAssignment Combine(params FormationAssignment[] assignments)
        {
            var children = new Dictionary<SimpleFormationHandler, IFormationAssignment>();
            var units = new Dictionary<UnitHandler, IUnitAssignment>();
            foreach (var assignment in assignments)
            {
                AddAll(children, assignment.ChildFormationAssignments);
                AddAll(units, assignment.UnitAssignments);
            }
            return new(children, units);
        }

        public static void AddAll<TKey, TValue>(Dictionary<TKey, TValue> sink, Dictionary<TKey, TValue> source)
            where TKey : notnull
        {
            foreach (var kvp in source)
            {
                sink.Add(kvp.Key, kvp.Value);
            }
        }
    }
}
