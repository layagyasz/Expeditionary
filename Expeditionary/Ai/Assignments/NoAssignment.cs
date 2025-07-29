using Expeditionary.Ai.Actions;
using Expeditionary.Evaluation;
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

        public AssignmentRealization Assign(IAiHandler formation, Match match, TileEvaluator tileEvaluator)
        {
            return new(
                formation.Children.ToDictionary(x => x, x => (IAssignment)new NoAssignment(Origin)),
                formation.Diads.ToDictionary(x => x, x => (IAssignment)new NoAssignment(Origin)));
        }

        public float EvaluateAction(
            Unit unit, IUnitAction action, TileEvaluator.UnitTileEvaluator tileEvaluator, Match match)
        {
            throw new NotImplementedException();
        }

        public float EvaluateRealization(AssignmentRealization realization, Match match)
        {
            return realization.ChildFormationAssignments.All(x => x.Value is NoAssignment)
                && realization.UnitAssignments.All(x => x.Value is NoAssignment) ? 1f : 0f;
        }

        public Vector3i SelectHex(Map map)
        {
            throw new NotImplementedException();
        }
    }
}
