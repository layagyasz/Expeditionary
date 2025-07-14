using Expeditionary.Ai.Actions;
using Expeditionary.Evaluation;
using Expeditionary.Evaluation.Considerations;
using Expeditionary.Evaluation.SignedDistanceFields;
using Expeditionary.Hexagons;
using Expeditionary.Model;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Mapping.Regions;
using Expeditionary.Model.Units;
using OpenTK.Mathematics;
using static Expeditionary.Evaluation.TileEvaluator;

namespace Expeditionary.Ai.Assignments
{
    public record class PointAssignment(Vector3i Hex, IMapRegion Bounds, MapDirection Facing) : IAssignment
    {
        private static readonly float s_Reward = 20f;

        public static AssignmentRealization SelectFrom(
            IAiHandler formation,
            Map map,
            IMapRegion region,
            MapDirection facing,
            TileEvaluator tileEvaluator,
            TileConsideration extraConsideration)
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
                        spacing))
                .AddAll(SelectFrom(formation.Diads, map, region, facing, tileEvaluator, extraConsideration, spacing))
                .Build();
        }

        public static AssignmentRealization SelectFrom(
            IEnumerable<FormationHandler> formations,
            Map map,
            IMapRegion region,
            MapDirection facing,
            TileEvaluator tileEvaluator,
            TileConsideration extraConsideration,
            int spacing)
        {
            var sdf = new CompositeSignedDistanceField();
            var result = new Dictionary<FormationHandler, IAssignment>();
            foreach (var formation in formations)
            {
                var consideration =
                    TileConsiderations.Combine(
                        tileEvaluator.GetConsiderationFor(formation.Formation, facing),
                        extraConsideration,
                        TileConsiderations.Exterior(sdf, 0));
                var hex = AssignmentHelper.GetBest(map, region, consideration);
                result.Add(formation, new PointAssignment(hex, region, facing));
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
            int spacing)
        {
            var sdf = new CompositeSignedDistanceField();
            var result = new Dictionary<DiadHandler, IAssignment>();
            foreach (var unit in diads.OrderBy(x => -x.Unit.Unit.Type.Points))
            {
                var consideration =
                    TileConsiderations.Combine(
                        tileEvaluator.GetConsiderationFor(unit.Role, unit.Unit.Unit.Type, facing),
                        extraConsideration,
                        TileConsiderations.Exterior(sdf, 0));
                var hex = AssignmentHelper.GetBest(map, region, consideration);
                result.Add(unit, new PointAssignment(hex, region, facing));
                sdf.Add(new SimpleSignedDistanceField(hex, 0, spacing));
            }
            return new(new(), result);
        }

        public IMapRegion Region => Bounds;

        public AssignmentRealization Assign(IAiHandler formation, Match match, TileEvaluator tileEvaluator)
        {
            int spacing = GetSpacing(formation.Echelon);
            var supportRegion = CompositeMapRegion.Intersect(new PointMapRegion(Hex, 2 * spacing), Bounds);
            var result = new AssignmentRealization.Builder();
            if (formation.Children.Any())
            {
                var first = formation.Children.First();
                result.Add(first, new PointAssignment(Hex, Bounds, Facing));
                result.AddAll(
                    SelectFrom(
                        formation.Children.Skip(1),
                        match.GetMap(),
                        supportRegion,
                        Facing,
                        tileEvaluator,
                        TileConsiderations.Exterior(new SimpleSignedDistanceField(Hex, 0, spacing), 0),
                        spacing));
            }
            if (formation.Diads.Any())
            {
                if (formation.Children.Any())
                {
                    var diads = formation.Diads.ToList();
                    var first = diads.First();
                    result.Add(first, new PointAssignment(Hex, Bounds, Facing));
                    result.AddAll(
                        SelectFrom(
                            diads.Skip(1),
                            match.GetMap(),
                            supportRegion,
                            Facing,
                            tileEvaluator,
                            TileConsiderations.Exterior(new SimpleSignedDistanceField(Hex, 0, spacing), 0),
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
                            spacing));
                }
            }
            return result.Build();
        }

        public float EvaluateAction(Unit unit, IUnitAction action, UnitTileEvaluator tileEvaluator, Match match)
        {
            if (action is AttackAction attackAction)
            {
                return AttackEvaluation.Evaluate(
                    unit, attackAction.Attack, attackAction.Mode, attackAction.Target, match.GetMap());
            }
            if (action is MoveAction moveAction)
            {
                var consideration =
                    tileEvaluator.GetThreatConsiderationFor(Disposition.Defensive, match);
                var baseline = TileConsiderations.Evaluate(consideration, unit.Position!.Value, match.GetMap());
                if (unit.Position != Hex)
                {
                    var inDirection =
                        Geometry.GetCubicDistance(unit.Position!.Value, Hex)
                        > Geometry.GetCubicDistance(moveAction.Path.Destination, Hex) ? 1 : 0;
                    return unit.UnitQuantity.Points
                        * (TileConsiderations.Evaluate(
                            consideration, moveAction.Path.Destination, match.GetMap()) - baseline) +
                            s_Reward * (moveAction.Path.Cost / unit.Type.Speed + inDirection);
                }
                else
                {
                    return unit.UnitQuantity.Points
                        * (TileConsiderations.Evaluate(
                            consideration, moveAction.Path.Destination, match.GetMap()) - baseline) - s_Reward;
                }
            }
            if (action is IdleAction)
            {
                return unit.Position == Hex ? s_Reward : 0;
            }
            throw new ArgumentException($"Unsupported action {action}");
        }

        public float EvaluateRealization(AssignmentRealization realization, Match match)
        {
            return 0;
        }

        public void Place(UnitHandler unit, Match match)
        {
            match.Place(unit.Unit, Hex);
        }


        private static int GetSpacing(int echelon)
        {
            if (echelon < 3)
            {
                return 1;
            }
            return (int)Math.Pow(3, echelon - 2);
        }
    }
}
