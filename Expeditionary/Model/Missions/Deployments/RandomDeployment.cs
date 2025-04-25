using Expeditionary.Evaluation;
using Expeditionary.Model.Formations;
using Expeditionary.Model.Mapping.Regions;

namespace Expeditionary.Model.Missions.Deployments
{
    public record class RandomDeployment(IMapRegion DeploymentRegion) : IDeployment
    {
        public void Setup(Formation formation, Match match, EvaluationCache evaluationCache, Random random)
        {
            var map = match.GetMap();
            var options = DeploymentRegion.Range(map).Where(x => !map.Get(x)!.Terrain.IsLiquid).ToList();
            foreach ((var unit, var _) in formation.GetUnitsAndRoles())
            {
                match.Place(unit, options[random.Next(options.Count)]);
            }
        }
    }
}
