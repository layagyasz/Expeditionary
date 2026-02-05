using Expeditionary.Model.Mapping;
using Expeditionary.Model.Units;

namespace Expeditionary.Model.Missions.Objectives
{
    public record class OccupyObjective(MapTag Area, AssetValue OccupationQuantity, bool IsDefender) : IObjective
    {
        public ObjectiveCompletion Evaluate(Player player, Match match)
        {
            var units = match.GetAssetsIn(Area).Where(x => x is Unit).Cast<Unit>();
            var enemy = 
                units.Where(x => !player.MatchTeam(x.Player)).Select(x => x.Value).Aggregate((x, y) => x + y);
            var friendly =
                units.Where(x => player.MatchTeam(x.Player)).Select(x => x.Value).Aggregate((x, y) => x + y);
            var disposition = IsDefender ? ObjectiveDisposition.Pessimistic : ObjectiveDisposition.Optimistic;
            if (enemy.Number == 0 && friendly >= OccupationQuantity)
            {
                return IObjective.WrapDefault(disposition, ObjectiveStatus.DecisiveVictory);
            }
            if (enemy.Number > 0 && friendly >= OccupationQuantity)
            {
                return IObjective.WrapDefault(disposition, ObjectiveStatus.MarginalVictory);
            }
            if (enemy.Number == 0 && friendly.Number > 0)
            {
                return IObjective.WrapDefault(disposition, ObjectiveStatus.Stalemate);
            }
            if (enemy.Number > 0 && friendly.Number > 0)
            {
                return IObjective.WrapDefault(disposition, ObjectiveStatus.MarginalDefeat);
            }
            return IObjective.WrapDefault(disposition, ObjectiveStatus.DecisiveDefeat);
        }
    }
}
