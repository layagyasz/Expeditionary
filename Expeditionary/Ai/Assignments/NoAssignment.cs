using Expeditionary.Ai.Actions;
using Expeditionary.Model;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Mapping.Regions;
using Expeditionary.Model.Units;
using OpenTK.Mathematics;

namespace Expeditionary.Ai.Assignments
{
    public record class NoAssignment(Vector3i Origin) : IAssignment
    {
        public MapDirection Facing => MapDirection.All;
        public IMapRegion Region => MapRegions.Empty;

        public AssignmentRealization Assign(IAiHandler formation, Match match)
        {
            return new(
                formation.Children.ToDictionary(x => x, x => (IAssignment)new NoAssignment(Origin)),
                formation.Diads.ToDictionary(x => x, x => (IAssignment)new NoAssignment(Origin)));
        }

        public IEnumerable<float> EvaluateActions(IEnumerable<IUnitAction> actions, Unit unit, Match match)
        {
            return Enumerable.Repeat(0f, actions.Count());
        }

        public float EvaluateRealization(AssignmentRealization realization, Match match)
        {
            return realization.ChildFormationAssignments.All(x => x.Value is NoAssignment)
                && realization.UnitAssignments.All(x => x.Value is NoAssignment) ? 1f : 0f;
        }

        public bool NotifyAction(Unit unit, IUnitAction action, Match match)
        {
            return true;
        }

        public Vector3i SelectHex(Map map)
        {
            throw new NotImplementedException();
        }
    }
}
