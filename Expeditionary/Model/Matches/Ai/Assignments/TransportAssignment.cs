using Expeditionary.Model.Mapping;
using Expeditionary.Model.Mapping.Regions;
using Expeditionary.Model.Matches.Ai.Actions;
using Expeditionary.Model.Matches.Assets;
using Expeditionary.Model.Matches.Evaluation;
using Expeditionary.Model.Matches.Evaluation.Considerations;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Matches.Ai.Assignments
{
    public record class TransportAssignment(MatchUnit Unit, Vector3i Hex, Vector3i Origin, MapDirection Facing)
        : IAssignment
    {
        private static readonly float Reward = 2f;

        public IMapRegion Region => new PointMapRegion(Hex, 0);

        public AssignmentRealization Assign(IAiHandler formation, Match match)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<float> EvaluateActions(IEnumerable<IUnitAction> actions, MatchUnit unit, Match match)
        {
            var path = GetPath(unit, match);
            return actions.Select(x => EvaluateAction(x, unit, path));
        }

        public float EvaluateRealization(AssignmentRealization realization, Match match)
        {
            return 0;
        }

        public bool NotifyAction(MatchUnit unit, IUnitAction action, Match match)
        {
            return action is UnloadAction || !Unit.IsActive;
        }

        public Vector3i SelectHex(Map map)
        {
            throw new NotImplementedException();
        }

        private Pathing.Path? GetPath(MatchUnit unit, Match match)
        {
            if (!Unit.IsActive)
            {
                return null;
            }
            if (unit.Passenger != Unit && unit.Position != Unit.Position)
            {
                return Pathing.GetShortestPath(
                    match.GetMap(), unit.Position, Unit.Position, unit.Type.Movement, TileConsiderations.None);
            }
            if (unit.Passenger == Unit && Unit.Position != Hex)
            {
                return Pathing.GetShortestPath(
                    match.GetMap(), unit.Position, Hex, unit.Type.Movement, TileConsiderations.None);
            }
            return null;
        }

        private float EvaluateAction(IUnitAction action, MatchUnit unit, Pathing.Path? path)
        {
            var reward = Reward * Unit.Value.Points;
            if (action is MoveAction moveAction && path != null)
            {
                return ActionEvaluation.EvaluateMovePathBonus(unit, moveAction.Path, path);
            }
            if (unit.Passenger != Unit && unit.Position == Unit.Position)
            {
                if (action is LoadAction)
                {
                    return reward;
                }
            }
            if (unit.Passenger == Unit && unit.Position == Hex)
            {
                if (action is UnloadAction)
                {
                    return reward;
                }
                return -reward;
            }
            return 0;
        }
    }
}
