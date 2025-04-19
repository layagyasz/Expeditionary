using Expeditionary.Model.Formations;

namespace Expeditionary.Model.Missions.Deployments
{
    public interface IDeployment
    {
        void Setup(IEnumerable<Formation> formation, Match match, PlayerSetupContext context);
    }
}
