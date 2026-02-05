namespace Expeditionary.Model.Missions.Objectives
{
    public interface IObjective
    {
        ObjectiveCompletion Evaluate(Player player, Match match);

        public static ObjectiveDisposition Combine(IEnumerable<ObjectiveDisposition> dispositions)
        {
            if (dispositions.All(disposition => disposition == ObjectiveDisposition.Optimistic))
            {
                return ObjectiveDisposition.Optimistic;
            }
            if (dispositions.All(disposition => disposition == ObjectiveDisposition.Pessimistic))
            {
                return ObjectiveDisposition.Pessimistic;
            }
            return ObjectiveDisposition.Mixed;
        }

        public static ObjectiveCompletion WrapDefault(ObjectiveDisposition disposition, ObjectiveStatus status)
        {
            return new(status, disposition, IsTerminalDefault(disposition, status));
        }

        public static bool IsTerminalDefault(ObjectiveDisposition disposition, ObjectiveStatus status)
        {
            return disposition switch
            {
                ObjectiveDisposition.Optimistic => Optimistic(status),
                ObjectiveDisposition.Pessimistic => Pessimistic(status),
                _ => throw new ArgumentException($"Unsupported ObjectiveStatus {status}"),
            };
        }

        public static bool Optimistic(ObjectiveStatus status)
        {
            return status == ObjectiveStatus.DecisiveVictory;
        }

        public static bool Pessimistic(ObjectiveStatus status)
        {
            return status == ObjectiveStatus.DecisiveDefeat;
        }
    }
}
