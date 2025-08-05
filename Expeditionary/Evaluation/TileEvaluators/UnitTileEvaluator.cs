using Expeditionary.Evaluation.Caches;
using Expeditionary.Evaluation.Considerations;
using Expeditionary.Model;
using Expeditionary.Model.Knowledge;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Units;

namespace Expeditionary.Evaluation.TileEvaluators
{
    public class UnitTileEvaluator : PlayerTileEvaluator
    {
        private readonly Unit _unit;
        private readonly MapDirection _facing;
        private readonly RangeBucket _range;

        public UnitTileEvaluator(
            Unit unit,
            MapDirection facing,
            RangeBucket range, 
            IPlayerKnowledge knowledge,
            EvaluationCache evaluationCache, 
            Random random)
            : base(knowledge, evaluationCache, random)
        {
            _unit = unit;
            _facing = facing;
            _range = range;
        }

        public UnitTileEvaluator ForPassenger(Unit passenger)
        {
            return new(passenger, _facing, _range, _knowledge, _evaluationCache, _random);
        }

        public TileConsideration GetThreatConsiderationFor(Disposition disposition, Match match)
        {
            return TileConsiderations.Subtract(
                DefaultConsideration(
                    disposition, _facing, disposition == Disposition.Offensive ? _range : RangeBucket.Medium),
                TileConsiderations.Threat(_unit, _knowledge, match));
        }
    }
}
