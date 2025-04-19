using Expeditionary.Model.Formations;
using Expeditionary.Model.Missions.Deployments;

namespace Expeditionary.Model.Missions
{
    public record class FormationSetup(FormationTemplate Formation, IDeployment Deployment)
    {
        public void Setup(Player player, Match match, PlayerSetupContext context)
        {
            var formation = match.Add(player, Formation);
            Deployment.Setup(Enumerable.Repeat(formation, 1), match, context);
        }
    }
}
