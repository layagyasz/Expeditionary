using Expeditionary.Model.Formations;
using Expeditionary.Model.Mapping.Regions;

namespace Expeditionary.Model.Missions.Deployments
{
    public record class RandomDeployment(IMapRegion DeploymentRegion) : IDeployment
    {
        public void Setup(IEnumerable<Formation> formations, Match match, PlayerSetupContext context)
        {
            var map = match.GetMap();
            var options = DeploymentRegion.Range(map).Where(x => !map.Get(x)!.Terrain.IsLiquid).ToList();
            foreach ((var unit, var _) in formations.SelectMany(x => x.GetUnitsAndRoles()))
            {
                match.Place(unit, options[context.Parent.Random.Next(options.Count)]);
            }
        }
    }
}
