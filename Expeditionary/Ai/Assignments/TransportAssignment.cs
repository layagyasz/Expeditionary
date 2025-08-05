using Expeditionary.Ai.Actions;
using Expeditionary.Evaluation.TileEvaluators;
using Expeditionary.Evaluation;
using Expeditionary.Model;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Mapping.Regions;
using Expeditionary.Model.Units;
using OpenTK.Mathematics;
using Expeditionary.Evaluation.Considerations;

namespace Expeditionary.Ai.Assignments
{
    public record class TransportAssignment(Unit Unit, Vector3i Hex, Vector3i Origin, MapDirection Facing) 
        : IAssignment
    {
        private static readonly float s_Reward = 20f;

        public IMapRegion Region => new PointMapRegion(Hex, 0);

        public AssignmentRealization Assign(IAiHandler formation, Match match)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<(IUnitAction, float)> EvaluateActions(
            IEnumerable<IUnitAction> actions, Unit unit, Match match)
        {
            var evaluator = match.GetEvaluatorFor(unit, Facing);
            var path = GetPath(unit, match);
            return actions.Select(x => (x, EvaluateAction(x, unit, evaluator, match, path)));
        }

        public float EvaluateRealization(AssignmentRealization realization, Match match)
        {
            return 0;
        }

        public bool NotifyAction(Unit unit, IUnitAction action, Match match)
        {
            return action is UnloadAction;
        }

        public Vector3i SelectHex(Map map)
        {
            throw new NotImplementedException();
        }

        private Pathing.Path? GetPath(Unit unit, Match match)
        {
            if (unit.Passenger != Unit && unit.Position!.Value != Unit.Position!.Value)
            {
                return Pathing.GetShortestPath(
                    match.GetMap(),
                    unit.Position!.Value,
                    Unit.Position!.Value,
                    unit.Type.Movement,
                    TileConsiderations.None);
            }
            if (unit.Passenger == Unit && Unit.Position!.Value != Hex)
            {
                return Pathing.GetShortestPath(
                    match.GetMap(),
                    unit.Position!.Value,
                    Hex,
                    unit.Type.Movement,
                    TileConsiderations.None);
            }
            return null;
        }

        private float EvaluateAction(
            IUnitAction action, Unit unit, UnitTileEvaluator tileEvaluator, Match match, Pathing.Path? path)
        {
            if (action is MoveAction moveAction && path != null)
            {
                return ActionEvaluation.EvaluatePathMove(unit, moveAction.Path, path, match, tileEvaluator);
            }
            var defaultEval = UnitActionEvaluations.EvaluateDefault(action, unit, match, tileEvaluator);
            if (unit.Passenger != Unit && unit.Position!.Value == Unit.Position!.Value)
            {
                if (action is LoadAction)
                {
                    return defaultEval + s_Reward;
                }
            }
            if (unit.Passenger == Unit && unit.Position!.Value == Hex)
            {
                if (action is UnloadAction)
                {
                    return defaultEval + s_Reward;
                }
                return defaultEval - s_Reward;
            }
            return 0;
        }
    }
}
