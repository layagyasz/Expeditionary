namespace Expeditionary.Ai.Assignments
{
    public record class AssignmentRealization(
        Dictionary<SimpleFormationHandler, IAssignment> ChildFormationAssignments, 
        Dictionary<UnitHandler, IAssignment> UnitAssignments)
    {
        public class Builder
        {
            private readonly Dictionary<SimpleFormationHandler, IAssignment> _childFormationAssignments = 
                new();
            private readonly Dictionary<UnitHandler, IAssignment> _unitAssignments = new();

            public Builder Add(SimpleFormationHandler formation, IAssignment assignment)
            {
                _childFormationAssignments.Add(formation, assignment);
                return this;
            }

            public Builder Add(UnitHandler unit, IAssignment assignment)
            {
                _unitAssignments.Add(unit, assignment);
                return this;
            }

            public Builder AddAll(AssignmentRealization other)
            {
                AddAll(_childFormationAssignments, other.ChildFormationAssignments);
                AddAll(_unitAssignments, other.UnitAssignments);
                return this;
            }

            public AssignmentRealization Build()
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
