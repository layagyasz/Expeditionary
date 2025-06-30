using Expeditionary.Ai.Assignments.Units;
using Expeditionary.Evaluation;
using Expeditionary.Evaluation.Considerations;
using Expeditionary.Evaluation.SignedDistanceFields;
using Expeditionary.Model;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Mapping.Regions;
using OpenTK.Mathematics;

namespace Expeditionary.Ai.Assignments.Formations
{
    public record class PointAssignment(Vector3i Hex, IMapRegion Bounds, MapDirection Facing) : IFormationAssignment
    {
        public static FormationAssignment SelectFrom(
            IFormationHandler formation,
            Map map, 
            IMapRegion region,
            MapDirection facing,
            TileEvaluator tileEvaluator, 
            TileConsideration extraConsideration)
        {
            var spacing = 2 * GetSpacing(formation.Echelon);
            return new FormationAssignment.Builder()
                .AddAll(
                    SelectFrom(
                        formation.Children,
                        map, 
                        region,
                        facing,
                        tileEvaluator,
                        extraConsideration,
                        spacing))
                .AddAll(
                    SelectFrom(
                        formation.GetUnitHandlers(), map, region, facing, tileEvaluator, extraConsideration, spacing))
                .Build();
        }

        public static FormationAssignment SelectFrom(
            IEnumerable<SimpleFormationHandler> formations, 
            Map map,
            IMapRegion region, 
            MapDirection facing, 
            TileEvaluator tileEvaluator,
            TileConsideration extraConsideration, 
            int spacing)
        {
            var sdf = new CompositeSignedDistanceField();
            var result = new Dictionary<SimpleFormationHandler, IFormationAssignment>();
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

        public static FormationAssignment SelectFrom(
            IEnumerable<UnitHandler> units,
            Map map, 
            IMapRegion region,
            MapDirection facing,
            TileEvaluator tileEvaluator,
            TileConsideration extraConsideration, 
            int spacing)
        {
            var sdf = new CompositeSignedDistanceField();
            var result = new Dictionary<UnitHandler, IUnitAssignment>();
            foreach (var unit in units.OrderBy(x => -x.Unit.Type.Points))
            {
                var consideration = 
                    TileConsiderations.Combine(
                        tileEvaluator.GetConsiderationFor(unit.Role, unit.Unit.Type, facing),
                        extraConsideration,
                        TileConsiderations.Exterior(sdf, 0));
                var hex = AssignmentHelper.GetBest(map, region, consideration);
                result.Add(unit, new PositionAssignment(facing, hex));
                sdf.Add(new SimpleSignedDistanceField(hex, 0, spacing));
            }
            return new(new(), result);
        }

        public IMapRegion OperatingRegion => Bounds;

        public FormationAssignment Assign(IFormationHandler formation, Match match, TileEvaluator tileEvaluator)
        {
            int spacing = GetSpacing(formation.Echelon);
            var supportRegion = CompositeMapRegion.Intersect(new PointMapRegion(Hex, 2 * spacing), Bounds);
            var result = new FormationAssignment.Builder();
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
            if (formation.GetUnitHandlers().Any())
            {
                if (formation.Children.Any())
                {
                    var units = formation.GetUnitHandlers().ToList();
                    var first = units.First();
                    result.Add(first, new PositionAssignment(Facing, Hex));
                    result.AddAll(
                        SelectFrom(
                            units.Skip(1),
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
                            formation.GetUnitHandlers(),
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

        public float Evaluate(FormationAssignment assignment, Match match)
        {
            return 0;
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
