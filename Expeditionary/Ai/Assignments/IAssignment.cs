using Expeditionary.Ai.Actions;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Mapping.Regions;
using Expeditionary.Model.Matches;
using Expeditionary.Model.Matches.Assets;
using OpenTK.Mathematics;

namespace Expeditionary.Ai.Assignments
{
    public interface IAssignment
    {
        Vector3i Origin { get; }
        MapDirection Facing { get; }
        IMapRegion Region { get; }
        AssignmentRealization Assign(IAiHandler formation, Match match);
        IEnumerable<float> EvaluateActions(IEnumerable<IUnitAction> actions, Unit unit, Match match);
        float EvaluateRealization(AssignmentRealization realization, Match match);
        bool NotifyAction(Unit unit, IUnitAction action, Match match);
        Vector3i SelectHex(Map map);
    }
}
