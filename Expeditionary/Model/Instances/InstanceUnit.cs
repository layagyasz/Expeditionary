using Expeditionary.Model.Units;

namespace Expeditionary.Model.Instances
{
    public class InstanceUnit
    {
        public int Id { get; }
        public UnitType Type { get; }
        public InstanceUnitStatus Status { get; set; }

        public InstanceUnit(int id, UnitType type)
        {
            Id = id;
            Type = type;
        }
    }
}
