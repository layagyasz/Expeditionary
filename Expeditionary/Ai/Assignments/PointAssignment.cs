using Expeditionary.Ai.Actions;
using Expeditionary.Evaluation;
using Expeditionary.Evaluation.Considerations;
using Expeditionary.Evaluation.SignedDistanceFields;
using Expeditionary.Evaluation.TileEvaluators;
using Expeditionary.Model;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Mapping.Regions;
using Expeditionary.Model.Units;
using OpenTK.Mathematics;

namespace Expeditionary.Ai.Assignments
{
    public record class PointAssignment(Vector3i Hex, Vector3i Origin, IMapRegion Bounds, MapDirection Facing) 
        : IAssignment
    {
        private static readonly float s_Reward = 20f;

        public static AssignmentRealization SelectFrom(
            IAiHandler formation,
            Map map,
            IMapRegion region,
            MapDirection facing,
            TileEvaluator tileEvaluator,
            TileConsideration extraConsideration,
            Vector3i origin)
        {
            var spacing = 2 * GetSpacing(formation.Echelon);
            return new AssignmentRealization.Builder()
                .AddAll(
                    SelectFrom(
                        formation.Children,
                        map,
                        region,
                        facing,
                        tileEvaluator,
                        extraConsideration,
                        origin,
                        spacing))
                .AddAll(
                    SelectFrom(
                        formation.Diads, map, region, facing, tileEvaluator, extraConsideration, origin, spacing))
                .Build();
        }

        public static AssignmentRealization SelectFrom(
            IEnumerable<FormationHandler> formations,
            Map map,
            IMapRegion region,
            MapDirection facing,
            TileEvaluator tileEvaluator,
            TileConsideration extraConsideration,
            Vector3i origin,
            int spacing)
        {
            var sdf = new CompositeSignedDistanceField();
            var result = new Dictionary<FormationHandler, IAssignment>();
            foreach (var formation in formations)
            {
                var consideration =
                    TileConsiderations.Combine(
                        TileConsiderations.Essential(tileEvaluator.IsReachable(formation.GetMaxHindrance(), origin)),
                        tileEvaluator.GetConsiderationFor(formation.Formation, facing),
                        extraConsideration,
                        TileConsiderations.Exterior(sdf, 0));
                var hex = AssignmentHelper.GetBest(map, region, consideration);
                result.Add(formation, new PointAssignment(hex, origin, region, facing));
                sdf.Add(new SimpleSignedDistanceField(hex, 0, spacing));
            }
            return new(result, new());
        }

        public static AssignmentRealization SelectFrom(
            IEnumerable<DiadHandler> diads,
            Map map,
            IMapRegion region,
            MapDirection facing,
            TileEvaluator tileEvaluator,
            TileConsideration extraConsideration,
            Vector3i origin,
            int spacing)
        {
            var sdf = new CompositeSignedDistanceField();
            var result = new Dictionary<DiadHandler, IAssignment>();
            foreach (var diad in diads.OrderBy(x => -x.Unit.Unit.Type.Points))
            {
                var consideration =
                    TileConsiderations.Combine(
                        TileConsiderations.Essential(tileEvaluator.IsReachable(diad.GetMaxHindrance(), origin)),
                        tileEvaluator.GetConsiderationFor(diad.Role, diad.Unit.Unit.Type, facing),
                        extraConsideration,
                        TileConsiderations.Exterior(sdf, 0));
                var hex = AssignmentHelper.GetBest(map, region, consideration);
                result.Add(diad, new PointAssignment(hex, origin, region, facing));
                sdf.Add(new SimpleSignedDistanceField(hex, 0, spacing));
            }
            return new(new(), result);
        }

        public static PointAssignment GetShadowPoint(PointAssignment assignment, UnitHandler unit, Match match)
        {
            var tileEvaluator = match.GetEvaluator();
            return new PointAssignment(
                AssignmentHelper.GetBest(
                    match.GetMap(),
                    CompositeMapRegion.Intersect(
                        new PointMapRegion(assignment.Hex, 4),
                        new ExplicitMapRegion(
                            Pathing.GetPathField(
                                match.GetMap(),
                                assignment.Hex,
                                unit.Unit.Type.Movement, 
                                TileConsiderations.None, 
                                unit.Unit.Type.Speed)
                        .Select(x => x.Destination))),
                    TileConsiderations.Combine(
                        tileEvaluator.IsReachable(unit.GetMaxHindrance(), assignment.Hex),
                        tileEvaluator.GetConsiderationFor(unit.Role, unit.Unit.Type, assignment.Facing),
                        TileConsiderations.Direction(assignment.Hex, MapDirectionUtils.Invert(assignment.Facing)))),
                assignment.Origin,
                assignment.Bounds, 
                assignment.Facing);
        }

        public IMapRegion Region => Bounds;

        public AssignmentRealization Assign(IAiHandler formation, Match match)
        {
            int spacing = GetSpacing(formation.Echelon);
            var supportRegion = CompositeMapRegion.Intersect(new PointMapRegion(Hex, 2 * spacing), Bounds);
            var tileEvaluator = match.GetEvaluator();
            var result = new AssignmentRealization.Builder();
            if (formation.Children.Any())
            {
                var first = formation.Children.First();
                result.Add(first, new PointAssignment(Hex, Origin, Bounds, Facing));
                result.AddAll(
                    SelectFrom(
                        formation.Children.Skip(1),
                        match.GetMap(),
                        supportRegion,
                        Facing,
                        tileEvaluator,
                        TileConsiderations.Exterior(new SimpleSignedDistanceField(Hex, 0, spacing), 0),
                        Origin,
                        spacing));
            }
            if (formation.Diads.Any())
            {
                if (formation.Children.Any())
                {
                    var diads = formation.Diads.ToList();
                    var first = diads.First();
                    result.Add(first, new PointAssignment(Hex, Origin, Bounds, Facing));
                    result.AddAll(
                        SelectFrom(
                            diads.Skip(1),
                            match.GetMap(),
                            supportRegion,
                            Facing,
                            tileEvaluator,
                            TileConsiderations.Exterior(new SimpleSignedDistanceField(Hex, 0, spacing), 0),
                            Origin,
                            spacing));
                }
                else
                {
                    result.AddAll(
                        SelectFrom(
                            formation.Diads,
                            match.GetMap(),
                            supportRegion,
                            Facing,
                            tileEvaluator,
                            TileConsiderations.Exterior(new SimpleSignedDistanceField(Hex, 0, spacing), 0),
                            Origin,
                            spacing));
                }
            }
            return result.Build();
        }

        public IEnumerable<(IUnitAction, float)> EvaluateActions(
            IEnumerable<IUnitAction> actions, Unit unit, Match match)
        {
            var evaluator = match.GetEvaluatorFor(unit, Facing);
            var path = 
                unit.Position!.Value == Hex 
                ? null 
                : Pathing.GetShortestPath(
                    match.GetMap(), unit.Position!.Value, Hex, unit.Type.Movement, TileConsiderations.None);
            return actions.Select(x => (x, EvaluateAction(x, unit, evaluator, match, path)));
        }

        public float EvaluateRealization(AssignmentRealization realization, Match match)
        {
            return 0;
        }

        public bool NotifyAction(Unit unit, IUnitAction action, Match match)
        {
            if (action is MoveAction moveAction)
            {
                return moveAction.Path.Destination == Hex;
            }
            return unit.Position!.Value == Hex;
        }

        public Vector3i SelectHex(Map map)
        {
            return Hex;
        }

        public static int GetSpacing(int echelon)
        {
            if (echelon < 3)
            {
                return 1;
            }
            return (int)Math.Pow(3, echelon - 2);
        }

        private float EvaluateAction(
            IUnitAction action, Unit unit, UnitTileEvaluator tileEvaluator, Match match, Pathing.Path? path)
        {
            if (action is MoveAction moveAction && path != null)
            {
                return ActionEvaluation.EvaluatePathMove(unit, moveAction.Path, path, match, tileEvaluator);
            }
            var defaultEval = UnitActionEvaluations.EvaluateDefault(action, unit, match, tileEvaluator);
            if (unit.Position!.Value == Hex)
            {
                if (action is MoveAction)
                {
                    return defaultEval - s_Reward;
                }
                if (action is IdleAction)
                {
                    return defaultEval + s_Reward;
                }
            }
            return 0;
        }
    }
}
