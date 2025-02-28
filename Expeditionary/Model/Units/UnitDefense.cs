namespace Expeditionary.Model.Units
{
    public record class UnitDefense
    {
        public UnitBoundedValue Maneuver { get; set; }
        public UnitBoundedValue Armor { get; set; }
        public UnitBoundedValue Vitality { get; set; }
    }
}
