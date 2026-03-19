using Expeditionary.Evaluation.Caches;
using Expeditionary.Model.Matches.Knowledge;

namespace Expeditionary.Evaluation.TileEvaluators
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
