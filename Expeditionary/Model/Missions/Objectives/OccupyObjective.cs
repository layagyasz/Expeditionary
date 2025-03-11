using Expeditionary.Model.Mapping;
using Expeditionary.Model.Units;

namespace Expeditionary.Model.Missions.Objectives
{
    public record class OccupyObjective(MapTag Area, UnitQuantity OccupationQuantity) : IObjective
    {
        public ObjectiveStatus ComputeStatus(Player player, Match match)
        {
            var units = match.GetAssetsIn(Area).Where(x => x is Unit).Cast<Unit>();
            var enemy = 
                units.Where(x => !player.MatchTeam(x.Player)).Select(x => x.UnitQuantity).Aggregate((x, y) => x + y);
            var friendly =
                units.Where(x => player.MatchTeam(x.Player)).Select(x => x.UnitQuantity).Aggregate((x, y) => x + y);
            if (enemy.Number == 0 && friendly >= OccupationQuantity)
            {
                return ObjectiveStatus.DecisiveVictory;
            }
            if (enemy.Number > 0 && friendly >= OccupationQuantity)
            {
                return ObjectiveStatus.MarginalVictory;
            }
            if (enemy.Number == 0 && friendly.Number > 0)
            {
                return ObjectiveStatus.Stalemate;
            }
            if (enemy.Number > 0 && friendly.Number > 0)
            {
                return ObjectiveStatus.MarginalDefeat;
            }
            return ObjectiveStatus.DecisiveDefeat;
        }
    }
}
