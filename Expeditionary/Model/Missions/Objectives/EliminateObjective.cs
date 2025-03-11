namespace Expeditionary.Model.Missions.Objectives
{
    public record class EliminateObjective(UnitQuantity Quantity, bool Team) : IObjective
    {
        public ObjectiveStatus ComputeStatus(Player player, Match match)
        {
            var score = 
                Team 
                    ? match.GetStatistics(player.Team).Select(x => x.Destroyed).Aggregate((x, y) => x + y) 
                    : match.GetStatistics(player).Destroyed;
            if (score >= Quantity)
            {
                return ObjectiveStatus.DecisiveVictory;
            }
            if (score >= 0.75f * Quantity)
            {
                return ObjectiveStatus.MarginalVictory;
            }
            if (score >= 0.5f * Quantity)
            {
                return ObjectiveStatus.Stalemate;
            }
            if (score > 0.25f * Quantity)
            {
                return ObjectiveStatus.MarginalDefeat;
            }
            return ObjectiveStatus.DecisiveDefeat;
        }
    }
}
