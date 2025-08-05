namespace Expeditionary.Model.Missions
{
    public record class PlayerStatistics
    {
        public AssetValue Destroyed { get; set; }
        public AssetValue Lost { get; set; }

        public PlayerStatistics() { }

        public PlayerStatistics(AssetValue destroyed, AssetValue lost)
        {
            Destroyed = destroyed;
            Lost = lost;
        }
    }
}
