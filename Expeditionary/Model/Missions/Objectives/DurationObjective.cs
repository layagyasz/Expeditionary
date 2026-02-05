namespace Expeditionary.Model.Missions.Objectives
{
    public record class DurationObjective(int Turns) : IObjective
    {
        private static readonly ObjectiveDisposition s_Disposition = ObjectiveDisposition.Optimistic;

        public ObjectiveCompletion Evaluate(Player player, Match match)
        {
            int turn = match.GetTurn();
            if (turn >= Turns)
            {
                return IObjective.WrapDefault(s_Disposition, ObjectiveStatus.DecisiveVictory);
            }
            if (turn >= 0.83f * Turns)
            {
                return IObjective.WrapDefault(s_Disposition, ObjectiveStatus.MarginalVictory);
            }
            if (turn >= 0.67f * Turns)
            {
                return IObjective.WrapDefault(s_Disposition, ObjectiveStatus.Stalemate);
            }
            if (turn > 0.5f * Turns)
            {
                return IObjective.WrapDefault(s_Disposition, ObjectiveStatus.MarginalDefeat);
            }
            return IObjective.WrapDefault(s_Disposition, ObjectiveStatus.DecisiveDefeat);
        }
    }
}
