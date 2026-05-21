namespace Expeditionary.Model.Instances.Events
{
    public record class UpdateUnitStatusEvent(int InstanceUnitId, InstanceUnitStatus Status, int Number) 
        : IInstanceEvent
    {
        public bool Apply(GameInstance instance)
        {
            var unit = instance.Player.Formation.GetUnit(InstanceUnitId)!;
            unit.Status = Status;
            unit.Number = Number;
            return true;
        }
    }
}
