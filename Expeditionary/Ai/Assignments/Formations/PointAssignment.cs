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
            IFormationHandler formation, Map map, IMapRegion region, MapDirection facing, Random random)
        {
            return SelectFrom(
                formation.Children,
                map, 
                region,
                facing,
                DefaultConsideration(map, random),
                2 * GetSpacing(formation.Echelon));
        }

        public static FormationAssignment SelectFrom(
            IEnumerable<SimpleFormationHandler> formations, 
            Map map,
            IMapRegion region, 
            MapDirection facing, 
            TileConsideration consideration, 
            int spacing)
        {
            var sdf = new CompositeSignedDistanceField();
            consideration = TileConsiderations.Combine(consideration, TileConsiderations.Exterior(sdf, 0));
            var result = new Dictionary<SimpleFormationHandler, IFormationAssignment>();
            foreach (var formation in formations)
            {
                var hex = AssignmentHelper.GetBest(map, region, consideration);
                result.Add(formation, new PointAssignment(hex, facing));
                sdf.Add(new SimpleSignedDistanceField(hex, 0, spacing));
            }
            return new(result, new());
        }

        public static FormationAssignment SelectFrom(
            IEnumerable<UnitHandler> units, Map map, IMapRegion region, TileConsideration consideration, int spacing)
        {
            var sdf = new CompositeSignedDistanceField();
            consideration = TileConsiderations.Combine(consideration, TileConsiderations.Exterior(sdf, 0));
            var result = new Dictionary<UnitHandler, IUnitAssignment>();
            foreach (var unit in units)
            {
                var hex = AssignmentHelper.GetBest(map, region, consideration);
                result.Add(unit, new PositionAssignment(hex));
                sdf.Add(new SimpleSignedDistanceField(hex, 0, spacing));
            }
            return new(new(), result);
        }

        public FormationAssignment Assign(
            IFormationHandler formation, Match match, EvaluationCache evaluationCache, Random random)
        {
            int spacing = GetSpacing(formation.Echelon);
            var supportRegion = new PointMapRegion(Hex, 2 * spacing);
            var consideration = DefaultConsideration(match.GetMap(), random);
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
                        TileConsiderations.Combine(
                            consideration, 
                            TileConsiderations.Exterior(new SimpleSignedDistanceField(Hex, 0, spacing), 0)),
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
                        TileConsiderations.Combine(
                            consideration,
                            TileConsiderations.Exterior(new SimpleSignedDistanceField(Hex, 0, spacing), 0)),
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

        private static TileConsideration DefaultConsideration(Map map, Random random)
        {
            return TileConsiderations.Combine(
                TileConsiderations.Essential(TileConsiderations.Land),
                TileConsiderations.Forestation,
                TileConsiderations.Urbanization,
                TileConsiderations.Roading(map),
                TileConsiderations.Weight(0.1f, TileConsiderations.Noise(random)));
        }
    }
}
