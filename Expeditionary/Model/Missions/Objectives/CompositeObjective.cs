namespace Expeditionary.Model.Missions.Objectives
{
    public record class CompositeObjective(CompositeObjective.Operator Composition, List<IObjective> Objectives)
    {
        public enum Operator
        {
            And,
            Or
        }

        public ObjectiveCompletion Evaluate(Player player, Match match)
        {
            var completions = Objectives.Select(x => x.Evaluate(player, match)).ToList();
            var disposition = IObjective.Combine(completions.Select(completion => completion.Disposition));
            if (Composition == Operator.And)
            {
                return new ObjectiveCompletion(
                    completions.Min(completion => completion.Status),
                    disposition,
                    completions.All(completion => completion.IsTerminal)
                    || completions.Any(
                        completion => completion.Disposition == ObjectiveDisposition.Pessimistic 
                        && completion.IsTerminal));
            }
            else
            {
                return new ObjectiveCompletion(
                    completions.Max(completion => completion.Status),
                    disposition,
                    completions.All(completion => completion.IsTerminal)
                    || completions.Any(
                        completion => completion.Disposition == ObjectiveDisposition.Optimistic
                        && completion.IsTerminal));
            }
        }
    }
}
