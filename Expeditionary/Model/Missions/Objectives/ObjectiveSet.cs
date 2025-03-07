
namespace Expeditionary.Model.Missions.Objectives
{
    public class ObjectiveSet
    {
        private readonly List<IObjective> _objectives;

        public ObjectiveSet(IEnumerable<IObjective> objectives)
        {
            _objectives = objectives.ToList();
        }

        public ObjectiveStatus ComputeStatus(Player player, Match match)
        {
            return _objectives.Min(x => x.ComputeStatus(player, match));
        }
    }
}
