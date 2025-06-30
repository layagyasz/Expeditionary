using Expeditionary.Evaluation;
using Expeditionary.Model;
using Expeditionary.Model.Mapping.Regions;

namespace Expeditionary.Ai.Assignments.Formations
{
    public interface IFormationAssignment
    {
        IMapRegion OperatingRegion { get; }
        FormationAssignment Assign(IFormationHandler formation, Match match, TileEvaluator tileEvaluator);
        float Evaluate(FormationAssignment assignment, Match match);
    }
}
