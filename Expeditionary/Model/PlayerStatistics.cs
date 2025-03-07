namespace Expeditionary.Model
{
    public record class PlayerStatistics
    {
        public int DestroyedPoints { get; set; }
        public int DestroyedUnits { get; set; }
        public int LostPoints { get; set; }
        public int LostUnits { get; set; }
    }
}
