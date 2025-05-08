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
        public static FormationAssignment AssignInRegion(
            IFormationHandler formation,
            Match match,
            SignedDistanceField sdf,
            IMapRegion region,
            MapDirection facing,
            TileConsideration consideration,
            EvaluationCache evaluationCache)
        {
            var defensive = new List<UnitHandler>();
            var shortRange = new List<UnitHandler>();
            var medRange = new List<UnitHandler>();
            var longRange = new List<UnitHandler>();
            foreach (var unit in formation.GetUnitHandlers())
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

            var formationResult = new Dictionary<SimpleFormationHandler, IFormationAssignment>();
            var unitResult = new Dictionary<UnitHandler, IUnitAssignment>();

            foreach (var f in formation.GetAllFormationHandlers())
            {
                formationResult.Add(f, new AreaAssignment(region, facing));
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
                        evaluationCache.Exposure, facing, Disposition.Defensive, RangeBucket.Medium)),
                unitResult);
            Assign(
                match,
                shortRange,
                region,
                TileConsiderations.Combine(
                    consideration,
                    edge,
                    TileConsiderations.Exposure(
                        evaluationCache.Exposure, facing, Disposition.Offensive, RangeBucket.Short)),
                unitResult);
            Assign(
                match,
                medRange,
                region,
                TileConsiderations.Combine(
                    consideration,
                    edge,
                    TileConsiderations.Exposure(
                        evaluationCache.Exposure, facing, Disposition.Offensive, RangeBucket.Medium)),
                unitResult);
            Assign(
                match,
                longRange,
                region,
                TileConsiderations.Combine(
                    consideration,
                    edge,
                    TileConsiderations.Exposure(
                        evaluationCache.Exposure, facing, Disposition.Offensive, RangeBucket.Long)),
                unitResult);

            return new(formationResult, unitResult);
        }

        private static void Assign(
            Match match,
            IEnumerable<UnitHandler> units,
            IMapRegion region,
            TileConsideration consideration,
            Dictionary<UnitHandler, IUnitAssignment> result)
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
            foreach ((var unit, var hex) in units.Zip(tilesAndEvaluations).Select(x => (x.First, x.Second.hex)))
            {
                result.Add(unit, new PositionAssignment(hex));
            }
        }
    }
}
