using Expeditionary.Evaluation;
using Expeditionary.Model.Formations;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Mapping.Regions;

namespace Expeditionary.Model.Missions.Deployments
{
    public record class AreaDeployment(IMapRegion Region, MapDirection Facing) : IDeployment
    {
        public void Setup(FormationTemplate formation, Player player, Match match, PlayerSetupContext context)
        {
            DeploymentHelper.DeployInRegion(
                formation,
                player,
                match, 
                SignedDistanceField.FromRegion(match.GetMap(), Region, 0),
                Region, 
                Facing, 
                context);
        }
    }
}
