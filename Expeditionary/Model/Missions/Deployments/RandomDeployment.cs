using Expeditionary.Model.Formations;

namespace Expeditionary.Model.Missions.Deployments
{
    public class RandomDeployment : IDeployment
    {
        public void Setup(FormationTemplate formation, Player player, Match match, SetupContext context)
        {
            var map = match.GetMap();
            var options = map.Range().Where(x => !map.Get(x)!.Terrain.IsLiquid).ToList();
            foreach (var unit in formation.GetUnitTypesAndRoles())
            {
                match.Add(unit.UnitType, player, options[context.Random.Next(options.Count)]);
            }
        }
    }
}
