using Expeditionary.Ai.Assignments.Units;
using Expeditionary.Evaluation;
using Expeditionary.Evaluation.Considerations;
using Expeditionary.Model;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Mapping.Regions;

namespace Expeditionary.Ai.Assignments.Formations
{
    public static class AssignmentHelper
    {
        public static void DeployInRegion(
            FormationAssignment formation,
            Match match,
            SignedDistanceField sdf,
            IMapRegion region,
            MapDirection facing,
            TileConsideration consideration,
            EvaluationCache evaluationCache)
        {
            var defensive = new List<UnitAssignment>();
            var shortRange = new List<UnitAssignment>();
            var medRange = new List<UnitAssignment>();
            var longRange = new List<UnitAssignment>();
            foreach (var unit in formation.GetUnitAssignments())
            {
                if (DefaultDispositionMapper.Map(unit.Unit.Type) == Disposition.Defensive)
                {
                    defensive.Add(unit);
                    continue;
                }
                var range = RangeBucketizer.ToBucket(unit.Unit.Type);
                if (range == RangeBucket.Short)
                {
                    shortRange.Add(unit);
                }
                else if (range == RangeBucket.Medium)
                {
                    medRange.Add(unit);
                }
                else
                {
                    longRange.Add(unit);
                }
            }

            var edge = TileConsiderations.Edge(sdf, 0);
            Assign(
                match,
                defensive,
                region,
                TileConsiderations.Combine(
                    consideration,
                    TileConsiderations.Edge(sdf, -5),
                    TileConsiderations.Exposure(
                        evaluationCache.Exposure, facing, Disposition.Defensive, RangeBucket.Medium)));
            Assign(
                match,
                shortRange,
                region,
                TileConsiderations.Combine(
                    consideration,
                    edge,
                    TileConsiderations.Exposure(
                        evaluationCache.Exposure, facing, Disposition.Offensive, RangeBucket.Short)));
            Assign(
                match,
                medRange,
                region,
                TileConsiderations.Combine(
                    consideration,
                    edge,
                    TileConsiderations.Exposure(
                        evaluationCache.Exposure, facing, Disposition.Offensive, RangeBucket.Medium)));
            Assign(
                match,
                longRange,
                region,
                TileConsiderations.Combine(
                    consideration,
                    edge,
                    TileConsiderations.Exposure(
                        evaluationCache.Exposure, facing, Disposition.Offensive, RangeBucket.Long)));
        }

        private static void Assign(
            Match match,
            IEnumerable<UnitAssignment> units,
            IMapRegion region,
            TileConsideration consideration)
        {
            var tilesAndEvaluations =
                region.Range(match.GetMap())
                    .Select(hex =>
                        (
                            hex,
                            score: TileConsiderations.Evaluate(consideration, hex, match.GetMap())
                        ))
                    .ToList();
            while (tilesAndEvaluations.Count < units.Count())
            {
                tilesAndEvaluations.AddRange(tilesAndEvaluations);
            }
            tilesAndEvaluations.Sort((x, y) => -x.score.CompareTo(y.score));
            var assignments = units.Zip(tilesAndEvaluations).Select(x => (x.First, x.Second.hex));
            foreach (var (unit, hex) in assignments)
            {
                unit.SetAssignment(new PositionAssignment(hex));
            }
        }
    }
}
