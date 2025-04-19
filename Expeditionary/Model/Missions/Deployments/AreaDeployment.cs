using Expeditionary.Evaluation;
using Expeditionary.Evaluation.Considerations;
using Expeditionary.Model.Formations;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Mapping.Regions;

namespace Expeditionary.Model.Missions.Deployments
{
    public record class AreaDeployment(IMapRegion Region, MapDirection Facing) : IDeployment
    {
        public void Setup(IEnumerable<Formation> formations, Match match, PlayerSetupContext context)
        {
            DeploymentHelper.DeployInRegion(
                formations,
                match, 
                SignedDistanceField.FromRegion(match.GetMap(), Region, 0),
                Region, 
                Facing,
                TileConsiderations.Combine(
                    TileConsiderations.Essential(TileConsiderations.Land),
                    TileConsiderations.Forestation,
                    TileConsiderations.Urbanization,
                    TileConsiderations.Roading(match.GetMap()),
                    TileConsiderations.Weight(0.1f, TileConsiderations.Noise(context.Parent.Random))),
                context);
        }
    }
}
