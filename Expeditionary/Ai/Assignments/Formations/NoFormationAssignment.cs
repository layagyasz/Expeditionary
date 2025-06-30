using Expeditionary.Ai.Assignments.Units;
using Expeditionary.Evaluation;
using Expeditionary.Model;
using Expeditionary.Model.Mapping.Regions;

namespace Expeditionary.Ai.Assignments.Formations
{
    public class NoFormationAssignment : IFormationAssignment
    {
        public IMapRegion OperatingRegion => MapRegions.Empty;

        public FormationAssignment Assign(IFormationHandler formation, Match match, TileEvaluator tileEvaluator) 
        {
            return new(
                formation.Children.ToDictionary(x => x, x => (IFormationAssignment)new NoFormationAssignment()),
                formation.GetUnitHandlers().ToDictionary(x => x, x => (IUnitAssignment)new NoUnitAssignment()));
        }

        public float Evaluate(FormationAssignment assignment, Match match)
        {
            return assignment.ChildFormationAssignments.All(x => x.Value is NoFormationAssignment)
                && assignment.UnitAssignments.All(x => x.Value is NoUnitAssignment) ? 1f : 0f;
        }
    }
}
