using Expeditionary.Model.Formations;

namespace Expeditionary.Model.Missions.Deployments
{
    public interface IDeployment
    {
        void Setup(FormationTemplate formation, Player playere, Match match, SetupContext context);
    }
}
