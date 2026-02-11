namespace Expeditionary.Model.Missions.Objectives
{
    public record class DurationObjective(int Turns) : IObjective
    {
        private static readonly ObjectiveDisposition s_Disposition = ObjectiveDisposition.Optimistic;

        public ObjectiveCompletion Evaluate(Player player, Match match)
        {
            var p = GetProgress(player, match).GetPercentDone();
            if (p >= 1)
            {
                return IObjective.WrapDefault(s_Disposition, ObjectiveStatus.DecisiveVictory);
            }
            if (p >= 0.83f)
            {
                return IObjective.WrapDefault(s_Disposition, ObjectiveStatus.MarginalVictory);
            }
            if (p >= 0.67f)
            {
                return IObjective.WrapDefault(s_Disposition, ObjectiveStatus.Stalemate);
            }
            if (p > 0.5f)
            {
                return IObjective.WrapDefault(s_Disposition, ObjectiveStatus.MarginalDefeat);
            }
            return IObjective.WrapDefault(s_Disposition, ObjectiveStatus.DecisiveDefeat);
        }

        public ObjectiveProgress GetProgress(Player player, Match match)
        {
            return new(match.GetTurn(), Turns);
        }
    }
}
