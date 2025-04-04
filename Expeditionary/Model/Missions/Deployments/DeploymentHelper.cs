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
            FormationTemplate formation,
            Player player,
            Match match, 
            SignedDistanceField sdf,
            IMapRegion region, 
            MapDirection facing, 
            PlayerSetupContext context)
        {
            var defensive = new List<UnitType>();
            var shortRange = new List<UnitType>();
            var medRange = new List<UnitType>();
            var longRange = new List<UnitType>();
            foreach (var unit in formation.GetUnitTypesAndRoles())
            {
                if (DefaultDispositionMapper.Map(unit.UnitType) == Disposition.Defensive)
                {
                    defensive.Add(unit.UnitType);
                    continue;
                }
                var range = RangeBucketizer.ToBucket(unit.UnitType);
                if (range == RangeBucket.Short)
                {
                    shortRange.Add(unit.UnitType);
                }
                else if (range == RangeBucket.Medium)
                {
                    medRange.Add(unit.UnitType);
                }
                else
                {
                    longRange.Add(unit.UnitType);
                }
            }

            var edge = TileConsiderations.Edge(sdf, 0);
            Assign(
                match,
                player,
                defensive,
                region,
                TileConsiderations.Edge(sdf, -5),
                TileConsiderations.Exposure(context.ExposureCache, facing, Disposition.Defensive, RangeBucket.Medium),
                context.Parent.Random);
            Assign(
                match,
                player,
                shortRange,
                region,
                edge,
                TileConsiderations.Exposure(context.ExposureCache, facing, Disposition.Offensive, RangeBucket.Short),
                context.Parent.Random);
            Assign(
                match,
                player,
                medRange,
                region,
                edge,
                TileConsiderations.Exposure(context.ExposureCache, facing, Disposition.Offensive, RangeBucket.Medium),
                context.Parent.Random);
            Assign(
                match,
                player,
                longRange,
                region,
                edge,
                TileConsiderations.Exposure(context.ExposureCache, facing, Disposition.Offensive, RangeBucket.Long),
                context.Parent.Random);
        }

        private static void Assign(
            Match match,
            Player player,
            IEnumerable<UnitType> units,
            IMapRegion region,
            TileConsideration advancement,
            TileConsideration exposure,
            Random random)
        {
            var finalConsideration =
                TileConsiderations.Combine(
                    TileConsiderations.Essential(TileConsiderations.Land),
                    advancement,
                    exposure,
                    TileConsiderations.Forestation,
                    TileConsiderations.Urbanization,
                    TileConsiderations.Roading(match.GetMap()),
                    TileConsiderations.Weight(0.1f, TileConsiderations.Noise(random)));
            var tilesAndEvaluations =
                region.Range(match.GetMap())
                    .Select(hex =>
                        (
                            hex,
                            score: TileConsiderations.Evaluate(finalConsideration, hex, match.GetMap())
                        ))
                    .ToList();
            while (tilesAndEvaluations.Count < units.Count())
            {
                tilesAndEvaluations.AddRange(tilesAndEvaluations);
            }
            tilesAndEvaluations.Sort((x, y) => -x.score.CompareTo(y.score));
            var assignments = units.Zip(tilesAndEvaluations).Select(x => (x.First, x.Second.hex));
            foreach (var (unitType, hex) in assignments)
            {
                match.Add(unitType, player, hex);
            }
        }
    }
}
