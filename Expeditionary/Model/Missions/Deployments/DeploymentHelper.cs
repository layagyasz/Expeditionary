using Expeditionary.Evaluation;
using Expeditionary.Evaluation.Considerations;
using Expeditionary.Model.Formations;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Mapping.Regions;
using Expeditionary.Model.Units;

namespace Expeditionary.Model.Missions.Deployments
{
    public static class DeploymentHelper
    {
        public static void DeployInRegion(
            IEnumerable<Formation> formations,
            Match match, 
            SignedDistanceField sdf,
            IMapRegion region, 
            MapDirection facing, 
            TileConsideration consideration,
            PlayerSetupContext context)
        {
            var defensive = new List<Unit>();
            var shortRange = new List<Unit>();
            var medRange = new List<Unit>();
            var longRange = new List<Unit>();
            foreach ((var unit, var role) in formations.SelectMany(x => x.GetUnitsAndRoles()))
            {
                if (DefaultDispositionMapper.Map(unit.Type) == Disposition.Defensive)
                {
                    defensive.Add(unit);
                    continue;
                }
                var range = RangeBucketizer.ToBucket(unit.Type);
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
                        context.ExposureCache, facing, Disposition.Defensive, RangeBucket.Medium)));
            Assign(
                match,
                shortRange,
                region,
                TileConsiderations.Combine(
                    consideration,
                    edge,
                    TileConsiderations.Exposure(
                        context.ExposureCache, facing, Disposition.Offensive, RangeBucket.Short)));
            Assign(
                match,
                medRange,
                region,
                TileConsiderations.Combine(
                    consideration,
                    edge,
                    TileConsiderations.Exposure(
                        context.ExposureCache, facing, Disposition.Offensive, RangeBucket.Medium)));
            Assign(
                match,
                longRange,
                region,
                TileConsiderations.Combine(
                    consideration,
                    edge,
                    TileConsiderations.Exposure(
                        context.ExposureCache, facing, Disposition.Offensive, RangeBucket.Long)));
        }

        private static void Assign(
            Match match,
            IEnumerable<Unit> units,
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
                match.Place(unit, hex);
            }
        }
    }
}
