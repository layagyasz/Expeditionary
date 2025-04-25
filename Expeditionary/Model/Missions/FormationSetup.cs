using Expeditionary.Evaluation;
using Expeditionary.Model.Formations;
using Expeditionary.Model.Missions.Deployments;

namespace Expeditionary.Model.Missions
{
    public record class FormationSetup(FormationTemplate Formation, IDeployment Deployment)
    {
        public void Setup(Player player, Match match, SetupContext context, EvaluationCache evaluationCache)
        {
            var formation = match.Add(player, Formation);
            Deployment.Setup(formation, match, evaluationCache, context.Random);
        }
    }
}
