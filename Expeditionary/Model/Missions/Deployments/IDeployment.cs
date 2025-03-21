using Expeditionary.Model.Formations;

namespace Expeditionary.Model.Missions.Deployments
{
    public interface IDeployment
    {
        void Setup(FormationTemplate formation, Player player, Match match, SetupContext context);
    }
}
