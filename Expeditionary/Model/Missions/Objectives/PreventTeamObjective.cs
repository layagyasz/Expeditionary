namespace Expeditionary.Model.Missions.Objectives
{
    public record class PreventTeamObjective(int Team) : IObjective
    {
        public ObjectiveCompletion Evaluate(Player player, Match match)
        {
            var completions = match.GetObjectives(Team).Select(kvp => kvp.Item2.Evaluate(kvp.Item1, match)).ToList();
            return new ObjectiveCompletion(
                Invert(completions.Max(completion => completion.Status)), 
                IObjective.Combine(completions.Select(completion => completion.Disposition)),
                completions.All(completion => completion.IsTerminal) 
                || completions.Any(
                    completion => completion.Disposition == ObjectiveDisposition.Optimistic && completion.IsTerminal));
        }

        public ObjectiveProgress GetProgress(Player player, Match match)
        {
            var objectives = match.GetObjectives(Team).ToList();
            return new(
                objectives.Count(kvp => kvp.Item2.Evaluate(kvp.Item1, match).Status == ObjectiveStatus.DecisiveDefeat),
                objectives.Count);
        }

        private static ObjectiveStatus Invert(ObjectiveStatus status)
        {
            return status switch
            {
                ObjectiveStatus.DecisiveDefeat => ObjectiveStatus.DecisiveVictory,
                ObjectiveStatus.MarginalDefeat => ObjectiveStatus.MarginalVictory,
                ObjectiveStatus.Stalemate => ObjectiveStatus.Stalemate,
                ObjectiveStatus.MarginalVictory => ObjectiveStatus.MarginalDefeat,
                ObjectiveStatus.DecisiveVictory => ObjectiveStatus.DecisiveDefeat,
                _ => throw new ArgumentException($"Unhandled ObjectiveStatus {status}")
            };
        }
    }
}
