namespace Expeditionary.Model.Units
{
    public record class UnitIntrinsics
    {
        public Modifier Manpower { get; set; }
        public float Mass { get; set; }
        public Modifier Morale { get; set; }
        public Modifier Number { get; set; }
        public Modifier Power { get; set; }
        public Modifier Profile { get; set; }
        public UnitSpace Space { get; set; }
        public Modifier Stamina { get; set; }
    }
}
