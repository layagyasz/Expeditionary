using Expeditionary.Model.Formations;
using Expeditionary.Model.Mapping.Regions;

namespace Expeditionary.Model.Missions.Deployments
{
    public record class RandomDeployment(IMapRegion DeploymentRegion) : IDeployment
    {
        public void Setup(FormationTemplate formation, Player player, Match match, SetupContext context)
        {
            var map = match.GetMap();
            var options = DeploymentRegion.Range(map).Where(x => !map.Get(x)!.Terrain.IsLiquid).ToList();
            foreach (var unit in formation.GetUnitTypesAndRoles())
            {
                match.Add(unit.UnitType, player, options[context.Random.Next(options.Count)]);
            }
        }
    }
}
