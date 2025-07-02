using Expeditionary.Evaluation.Considerations;
using Expeditionary.Model;
using Expeditionary.Model.Formations;
using Expeditionary.Model.Knowledge;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Units;

namespace Expeditionary.Evaluation
{
    public class TileEvaluator
    {
        private readonly IPlayerKnowledge _knowledge;
        private readonly EvaluationCache _evaluationCache;
        private readonly Random _random;

        public TileEvaluator(IPlayerKnowledge knowledge, EvaluationCache evaluationCache, Random random) 
        { 
            _knowledge = knowledge;
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

        public UnitTileEvaluator GetEvaluatorFor(Unit unit, MapDirection facing)
        {
            return new UnitTileEvaluator(this, unit, facing, RangeBucketizer.ToBucket(unit.Type));
        }

        public class UnitTileEvaluator
        {
            private readonly TileEvaluator _tileEvaluator;
            private readonly Unit _unit;
            private readonly MapDirection _facing;
            private readonly RangeBucket _range;

            internal UnitTileEvaluator(TileEvaluator tileEvaluator, Unit unit, MapDirection facing, RangeBucket range)
            {
                _tileEvaluator = tileEvaluator;
                _unit = unit;
                _facing = facing;
                _range = range;
            }

            public TileConsideration GetThreatConsiderationFor(Disposition disposition, Match match)
            {
                return TileConsiderations.Subtract(
                    _tileEvaluator.DefaultConsideration(
                        disposition, _facing, disposition == Disposition.Offensive ? _range : RangeBucket.Medium), 
                    TileConsiderations.Threat(_unit, _tileEvaluator._knowledge, match));
            }
        }

        private TileConsideration DefaultConsideration(Disposition disposition, MapDirection facing, RangeBucket range)
        {
            return TileConsiderations.Combine(
                (1f, DefaultConsideration()),
                (2f, TileConsiderations.Exposure(_evaluationCache.Exposure, facing, disposition, range)));
        }

        private TileConsideration DefaultConsideration()
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
