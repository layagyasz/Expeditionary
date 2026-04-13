namespace Expeditionary.Model.Formations.Generator
{
    public record class DiadParameters
    {
        public int Number { get; set; }
        public UnitSlot Unit { get; set; } = new();
        public UnitSlot? Transport { get; set; }
    }
}
