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
    public record class PointAssignment(Vector3i Hex, MapDirection Facing) : IFormationAssignment
    {
        public static FormationAssignment SelectFrom(
            IFormationHandler formation,
            Map map, 
            IMapRegion region,
            MapDirection facing,
            TileEvaluator tileEvaluator, 
            TileConsideration extraConsideration)
        {
            return SelectFrom(
                formation.Children,
                map, 
                region,
                facing,
                tileEvaluator,
                extraConsideration,
                2 * GetSpacing(formation.Echelon));
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
                result.Add(formation, new PointAssignment(hex, facing));
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
            foreach (var unit in units)
            {
                var consideration = 
                    TileConsiderations.Combine(
                        tileEvaluator.GetConsiderationFor(unit.Role, unit.Unit.Type, facing),
                        extraConsideration,
                        TileConsiderations.Exterior(sdf, 0));
                var hex = AssignmentHelper.GetBest(map, region, consideration);
                result.Add(unit, new PositionAssignment(hex));
                sdf.Add(new SimpleSignedDistanceField(hex, 0, spacing));
            }
            return new(new(), result);
        }

        public FormationAssignment Assign(IFormationHandler formation, Match match, TileEvaluator tileEvaluator)
        {
            int spacing = GetSpacing(formation.Echelon);
            var supportRegion = new PointMapRegion(Hex, 2 * spacing);
            // Handle extra units
            if (formation.Children.Any())
            {
                var result = new Dictionary<SimpleFormationHandler, IFormationAssignment>();
                var first = formation.Children.First();
                result.Add(first, new PointAssignment(Hex, Facing));
                return FormationAssignment.Combine(
                    new(result, new()), 
                    SelectFrom(
                        formation.Children.Skip(1),
                        match.GetMap(),
                        supportRegion, 
                        Facing,
                        tileEvaluator,
                        TileConsiderations.Exterior(new SimpleSignedDistanceField(Hex, 0, spacing), 0),
                        spacing));
            } 
            else if (formation.GetUnitHandlers().Any())
            {
                var units = formation.GetUnitHandlers().ToList();
                var result = new Dictionary<UnitHandler, IUnitAssignment>();
                var first = units.First();
                result.Add(first, new PositionAssignment(Hex));
                return FormationAssignment.Combine(
                    new(new(), result),
                    SelectFrom(
                        units.Skip(1),
                        match.GetMap(),
                        supportRegion, 
                        Facing,
                        tileEvaluator,
                        TileConsiderations.Exterior(new SimpleSignedDistanceField(Hex, 0, spacing), 0),
                        spacing));
            }
            throw new ArgumentException($"Formation {formation} had no children or units.");
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
