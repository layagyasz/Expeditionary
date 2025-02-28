using Expeditionary.Model.Units;

namespace Expeditionary.Model.Knowledge
{
    public record class AssetKnowledgeChangedEventArgs
    {
        public Player Player { get; init; }
        public List<IAsset> Delta { get; init; }

        public AssetKnowledgeChangedEventArgs(Player player, List<IAsset> delta)
        {
            Player = player;
            Delta = delta;
        }
    }
}
