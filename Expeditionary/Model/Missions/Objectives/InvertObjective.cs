namespace Expeditionary.Model.Missions.Objectives
{
    public record class InvertObjective(int Team) : IObjective
    {
        public ObjectiveStatus ComputeStatus(Player player, Match match)
        {
            return Invert(match.GetObjectiveSets(Team).Select(x => x.ComputeStatus(player, match)).Max());
        }

        private static ObjectiveStatus Invert(ObjectiveStatus status)
        {
            switch (status)
            {
                case ObjectiveStatus.DecisiveDefeat:
                    return ObjectiveStatus.DecisiveVictory;
                case ObjectiveStatus.MarginalDefeat:
                    return ObjectiveStatus.MarginalVictory;
                case ObjectiveStatus.Stalemate:
                    return ObjectiveStatus.Stalemate;
                case ObjectiveStatus.MarginalVictory:
                    return ObjectiveStatus.MarginalDefeat;
                case ObjectiveStatus.DecisiveVictory:
                    return ObjectiveStatus.DecisiveDefeat;
                default:
                    return ObjectiveStatus.DecisiveDefeat;
            }
        }
    }
}
