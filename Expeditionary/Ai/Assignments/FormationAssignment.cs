using Expeditionary.Ai.Assignments.Formations;
using Expeditionary.Ai.Assignments.Units;

namespace Expeditionary.Ai.Assignments
{
    public record class FormationAssignment(
        Dictionary<SimpleFormationHandler, IFormationAssignment> ChildFormationAssignments, 
        Dictionary<UnitHandler, IUnitAssignment> UnitAssignments)
    {
        public class Builder
        {
            private readonly Dictionary<SimpleFormationHandler, IFormationAssignment> _childFormationAssignments = 
                new();
            private readonly Dictionary<UnitHandler, IUnitAssignment> _unitAssignments = new();

            public Builder Add(SimpleFormationHandler formation, IFormationAssignment assignment)
            {
                _childFormationAssignments.Add(formation, assignment);
                return this;
            }

            public Builder Add(UnitHandler unit, IUnitAssignment assignment)
            {
                _unitAssignments.Add(unit, assignment);
                return this;
            }

            public Builder AddAll(FormationAssignment other)
            {
                AddAll(_childFormationAssignments, other.ChildFormationAssignments);
                AddAll(_unitAssignments, other.UnitAssignments);
                return this;
            }

            public FormationAssignment Build()
            {
                return new(_childFormationAssignments, _unitAssignments);
            }

            private static void AddAll<TKey, TValue>(Dictionary<TKey, TValue> sink, Dictionary<TKey, TValue> source)
                where TKey : notnull
            {
                foreach (var kvp in source)
                {
                    sink.Add(kvp.Key, kvp.Value);
                }
            }
        }
    }
}
