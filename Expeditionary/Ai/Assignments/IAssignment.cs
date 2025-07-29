using Expeditionary.Ai.Actions;
using Expeditionary.Evaluation;
using Expeditionary.Model;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Mapping.Regions;
using Expeditionary.Model.Units;
using OpenTK.Mathematics;
using static Expeditionary.Evaluation.TileEvaluator;

namespace Expeditionary.Ai.Assignments
{
    public interface IAssignment
    {
        Vector3i Origin { get; }
        MapDirection Facing { get; }
        IMapRegion Region { get; }
        AssignmentRealization Assign(IAiHandler formation, Match match, TileEvaluator tileEvaluator);
        float EvaluateAction(Unit unit, IUnitAction action, UnitTileEvaluator tileEvaluator, Match match);
        float EvaluateRealization(AssignmentRealization realization, Match match);
        Vector3i SelectHex(Map map);
    }
}
