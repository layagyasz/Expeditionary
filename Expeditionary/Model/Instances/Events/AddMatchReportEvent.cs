namespace Expeditionary.Model.Instances.Events
{
    public record class AddMatchReportEvent(InstanceMatchReport Report) : IInstanceEvent
    {
        public bool Apply(GameInstance instance)
        {
            instance.AddReport(Report);
            return true;
        }
    }
}
