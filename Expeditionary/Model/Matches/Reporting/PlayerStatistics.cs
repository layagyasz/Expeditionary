namespace Expeditionary.Model.Matches.Reporting
{
    public record class PlayerStatistics
    {
        public AssetValue Destroyed { get; set; }
        public AssetValue Lost { get; set; }

        public PlayerStatistics() { }
    }
}
