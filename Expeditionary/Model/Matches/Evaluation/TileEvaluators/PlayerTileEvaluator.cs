using Expeditionary.Model.Matches.Evaluation.Caches;
using Expeditionary.Model.Matches.Knowledge;

namespace Expeditionary.Model.Matches.Evaluation.TileEvaluators
{
    public class PlayerTileEvaluator : TileEvaluator
    {
        protected readonly IPlayerKnowledge _knowledge;

        public PlayerTileEvaluator(IPlayerKnowledge knowledge, EvaluationCache evaluationCache, Random random)
            : base(evaluationCache, random)
        {
            _knowledge = knowledge;
        }
    }
}
