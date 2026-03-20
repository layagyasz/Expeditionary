namespace Expeditionary.Model.Matches
{
    public record class PlayerReport
    {
        public AssetValue Destroyed { get; set; }
        public AssetValue Lost { get; set; }

        public PlayerReport() { }
    }
}
