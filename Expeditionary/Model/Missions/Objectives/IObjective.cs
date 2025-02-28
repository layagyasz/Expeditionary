namespace Expeditionary.Model.Missions.Objectives
{
    public interface IObjective
    {
        IObjectiveTracker MakeTracker();
    }
}
