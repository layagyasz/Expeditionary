using Expeditionary.Evaluation;
using Expeditionary.Model.Formations;

namespace Expeditionary.Model.Missions.Deployments
{
    public interface IDeployment
    {
        void Setup(Formation formation, Match match, EvaluationCache evaluationCache, Random random);
    }
}
