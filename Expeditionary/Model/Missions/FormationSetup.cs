using Expeditionary.Model.Formations;
using Expeditionary.Model.Missions.Deployments;

namespace Expeditionary.Model.Missions
{
    public record class FormationSetup(FormationTemplate Formation, IDeployment Deployment)
    {
        public void Setup(Player player, Match match, SetupContext context)
        {
            Deployment.Setup(Formation, player, match, context);
        }
    }
}
