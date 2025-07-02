namespace Expeditionary.Ai.Assignments
{
    public record class AssignmentRealization(
        Dictionary<FormationHandler, IAssignment> ChildFormationAssignments, 
        Dictionary<DiadHandler, IAssignment> UnitAssignments)
    {
        public class Builder
        {
            private readonly Dictionary<FormationHandler, IAssignment> _childFormationAssignments = 
                new();
            private readonly Dictionary<DiadHandler, IAssignment> _unitAssignments = new();

            public Builder Add(FormationHandler formation, IAssignment assignment)
            {
                _childFormationAssignments.Add(formation, assignment);
                return this;
            }

            public Builder Add(DiadHandler diad, IAssignment assignment)
            {
                _unitAssignments.Add(diad, assignment);
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
