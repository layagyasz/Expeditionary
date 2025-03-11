namespace Expeditionary.Model.Missions
{
    public record class PlayerStatistics
    {
        public UnitQuantity Destroyed { get; set; }
        public UnitQuantity Lost { get; set; }

        public PlayerStatistics() { }

        public PlayerStatistics(UnitQuantity destroyed, UnitQuantity lost)
        {
            Destroyed = destroyed;
            Lost = lost;
        }
    }
}
