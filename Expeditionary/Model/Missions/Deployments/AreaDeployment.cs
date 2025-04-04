using Expeditionary.Evaluation;
using Expeditionary.Evaluation.Considerations;
using Expeditionary.Model.Formations;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Mapping.Regions;
using Expeditionary.Model.Units;

namespace Expeditionary.Model.Missions.Deployments
{
    public record class AreaDeployment(IMapRegion Region, MapDirection Facing) : IDeployment
    {
        public void Setup(FormationTemplate formation, Player player, Match match, PlayerSetupContext context)
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

            var sdf = SignedDistanceField.FromRegion(match.GetMap(), Region, 0);
            var edge = TileConsiderations.Edge(TileConsiderations.Distance(sdf, 1, 0));
            Assign(
                match, 
                player,
                defensive,
                Region, 
                TileConsiderations.Edge(TileConsiderations.Distance(sdf, 1, -5)), 
                TileConsiderations.Exposure(context.ExposureCache, Facing, Disposition.Defensive, RangeBucket.Medium),
                context.Parent.Random);
            Assign(
                match,
                player,
                shortRange,
                Region,
                edge,
                TileConsiderations.Exposure(context.ExposureCache, Facing, Disposition.Offensive, RangeBucket.Short),
                context.Parent.Random);
            Assign(
                match,
                player,
                medRange,
                Region,
                edge, 
                TileConsiderations.Exposure(context.ExposureCache, Facing, Disposition.Offensive, RangeBucket.Medium),
                context.Parent.Random);
            Assign(
                match,
                player,
                longRange,
                Region,                    
                edge,
                TileConsiderations.Exposure(context.ExposureCache, Facing, Disposition.Offensive, RangeBucket.Long),
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
