namespace Expeditionary.Model.Missions.Objectives
{
    public interface IObjective
    {
        ObjectiveStatus ComputeStatus(Player player, Match match);
    }
}
