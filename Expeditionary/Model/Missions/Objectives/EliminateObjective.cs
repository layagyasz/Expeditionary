namespace Expeditionary.Model.Missions.Objectives
{
    public record class EliminateObjective(AssetValue Quantity, bool Team) : IObjective
    {
        private static readonly ObjectiveDisposition s_Disposition = ObjectiveDisposition.Optimistic;

        public ObjectiveCompletion Evaluate(Player player, Match match)
        {
            var p = GetProgress(player, match).GetPercentDone();
            if (p >= 1)
            {
                return IObjective.WrapDefault(s_Disposition, ObjectiveStatus.DecisiveVictory);
            }
            if (p >= 0.75f)
            {
                return IObjective.WrapDefault(s_Disposition, ObjectiveStatus.MarginalVictory);
            }
            if (p >= 0.5f)
            {
                return IObjective.WrapDefault(s_Disposition, ObjectiveStatus.Stalemate);
            }
            if (p > 0.25f)
            {
                return IObjective.WrapDefault(s_Disposition, ObjectiveStatus.MarginalDefeat);
            }
            return IObjective.WrapDefault(s_Disposition, ObjectiveStatus.DecisiveDefeat);
        }

        public ObjectiveProgress GetProgress(Player player, Match match)
        {
            return new(
                ToScalar(
                    Team
                        ? match.GetStatistics(player.Team).Select(x => x.Destroyed).Aggregate((x, y) => x + y)
                        : match.GetStatistics(player).Destroyed),
                ToScalar(Quantity));
        }

        private static float ToScalar(AssetValue value)
        {
            if (value.Points > 0)
            {
                return value.Points;
            }
            return value.Number;
        }
    }
}
