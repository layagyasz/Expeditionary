using Expeditionary.Evaluation;
using Expeditionary.Model;
using Expeditionary.Model.Mapping.Regions;

namespace Expeditionary.Ai.Assignments.Formations
{
    public record class RandomAssignment(IMapRegion DeploymentRegion) : IFormationAssignment
    {
        public void Assign(FormationHandler formation, Match match, EvaluationCache evaluationCache, Random random)
        {
            var map = match.GetMap();
            var options = DeploymentRegion.Range(map).Where(x => !map.Get(x)!.Terrain.IsLiquid).ToList();
            foreach ((var unit, var _) in formation.Formation.GetUnitsAndRoles())
            {
                match.Place(unit, options[random.Next(options.Count)]);
            }
        }
    }
}
