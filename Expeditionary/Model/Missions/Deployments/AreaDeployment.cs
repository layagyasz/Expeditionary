using Expeditionary.Evaluation;
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

            Assign(
                match, 
                player,
                defensive,
                Region, 
                Facing, 
                Disposition.Defensive,
                RangeBucket.Medium,
                context.EvaluationCache,
                context.Parent.Random);
            Assign(
                match,
                player,
                shortRange,
                Region,
                Facing,
                Disposition.Offensive,
                RangeBucket.Short,
                context.EvaluationCache,
                context.Parent.Random);
            Assign(
                match,
                player,
                medRange,
                Region,
                Facing,
                Disposition.Offensive,
                RangeBucket.Medium,
                context.EvaluationCache,
                context.Parent.Random);
            Assign(
                match,
                player,
                longRange,
                Region,
                Facing,
                Disposition.Offensive,
                RangeBucket.Long,
                context.EvaluationCache,
                context.Parent.Random);
        }

        private static void Assign(
            Match match,
            Player player,
            IEnumerable<UnitType> units, 
            IMapRegion region, 
            MapDirection facing, 
            Disposition disposition, 
            RangeBucket range,
            EvaluationCache evaluationCache,
            Random random)
        {
            var tilesAndEvaluations =
                region.Range(evaluationCache.Map)
                    .Select(hex =>
                        (
                            hex, 
                            score: evaluationCache.Evaluate(hex, facing, disposition, range)
                                + 0.1f * random.NextSingle())
                        )
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
