using Expeditionary.Evaluation.Caches;
using Expeditionary.Evaluation.Considerations;
using Expeditionary.Model;
using Expeditionary.Model.Formations;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Units;
using OpenTK.Mathematics;

namespace Expeditionary.Evaluation.TileEvaluators
{
    public class TileEvaluator
    {
        protected readonly EvaluationCache _evaluationCache;
        protected readonly Random _random;

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
                formation.GetDiads()
                    .Where(x => x.Role == formation.Role)
                    .Select(x => RangeBucketizer.ToBucket(x.Unit.Type))
                    .DefaultIfEmpty(RangeBucket.Short)
                    .Max());
        }

        public TileConsideration GetConsiderationFor(FormationRole role, UnitType unitType, MapDirection facing)
        {
            return DefaultConsideration(
                DefaultDispositionMapper.Map(unitType), facing, RangeBucketizer.ToBucket(unitType));
        }

        public bool IsReachable(Movement.Hindrance maxHindrance, Vector3i origin, Vector3i hex)
        {
            return _evaluationCache.Partition.IsReachable(origin, hex, maxHindrance);
        }

        public TileConsideration IsReachable(Movement.Hindrance maxHindrance, Vector3i origin)
        {
            return TileConsiderations.Essential(
                TileConsiderations.IsReachable(_evaluationCache.Partition, maxHindrance, origin));
        }

        public TileConsideration DefaultConsideration(Disposition disposition, MapDirection facing, RangeBucket range)
        {
            return TileConsiderations.Combine(
                (1f, DefaultConsideration()),
                (2f, TileConsiderations.Exposure(_evaluationCache.Exposure, facing, disposition, range)));
        }

        public TileConsideration DefaultConsideration()
        {
            return TileConsiderations.Combine(
                (1f, TileConsiderations.Essential(TileConsiderations.Land)),
                (1f, TileConsiderations.Forestation),
                (1f, TileConsiderations.Urbanization),
                (1f, TileConsiderations.Roading),
                (0.1f, TileConsiderations.Noise(_random)));
        }
    }
}
