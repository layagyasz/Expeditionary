namespace Expeditionary.Model.Combat.Units
{
    public record struct UnitSpace
    {
        public Modifier Available { get; set; }
        public Modifier Used { get; set; }
    }
}
