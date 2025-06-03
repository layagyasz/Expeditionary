using Expeditionary.Evaluation.Considerations;
using Expeditionary.Model.Formations;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Units;

namespace Expeditionary.Evaluation
{
    public class TileEvaluator
    {
        private readonly EvaluationCache _evaluationCache;
        private readonly Random _random;

        public TileEvaluator(EvaluationCache evaluationCache, Random random) 
        { 
            _evaluationCache = evaluationCache;
            _random = random;
        }

        public TileConsideration GetConsiderationFor(Formation formation, MapDirection facing)
        {
            return DefaultConsideration(
                DefaultDispositionMapper.Map(formation.Role), 
                facing, 
                formation.GetUnitsAndRoles()
                    .Where(x => x.Item2 == formation.Role)
                    .Max(x => RangeBucketizer.ToBucket(x.Item1.Type)));
        }

        public TileConsideration GetConsiderationFor(FormationRole role, UnitType unitType, MapDirection facing)
        {
            return DefaultConsideration(
                DefaultDispositionMapper.Map(unitType), facing, RangeBucketizer.ToBucket(unitType));
        }

        private TileConsideration DefaultConsideration(Disposition disposition, MapDirection facing, RangeBucket range)
        {
            return TileConsiderations.Combine(
                DefaultConsideration(),
                TileConsiderations.Weight(
                    2f, TileConsiderations.Exposure(_evaluationCache.Exposure, facing, disposition, range)));
        }

        private TileConsideration DefaultConsideration()
        {
            return TileConsiderations.Combine(
                TileConsiderations.Essential(TileConsiderations.Land),
                TileConsiderations.Forestation,
                TileConsiderations.Urbanization,
                TileConsiderations.Roading,
                TileConsiderations.Weight(0.1f, TileConsiderations.Noise(_random)));
        }
    }
}
