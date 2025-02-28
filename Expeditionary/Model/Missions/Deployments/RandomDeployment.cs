using Expeditionary.Model.Formations;

namespace Expeditionary.Model.Missions.Deployments
{
    public class RandomDeployment : IDeployment
    {
        public void Setup(FormationTemplate formation, Player player, Match match, SetupContext context)
        {
            var map = match.GetMap();
            var options = map.GetTiles().Where(x => !map.GetTile(x)!.Terrain.IsLiquid).ToList();
            foreach (var unit in formation.GetUnitTypes())
            {
                match.Add(unit, player, options[context.Random.Next(options.Count)]);
            }
        }
    }
}
