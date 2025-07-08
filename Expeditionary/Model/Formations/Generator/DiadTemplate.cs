namespace Expeditionary.Model.Formations.Generator
{
    public record class DiadTemplate
    {
        public int Number { get; set; }
        public UnitSlot Unit { get; set; } = new();
        public UnitSlot? Transport { get; set; }
    }
}
