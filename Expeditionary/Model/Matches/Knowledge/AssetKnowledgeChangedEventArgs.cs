using Expeditionary.Model.Matches.Assets;

namespace Expeditionary.Model.Matches.Knowledge
{
    public record class AssetKnowledgeChangedEventArgs(Player Player, List<IMatchAsset> Delta)
    {
        public override string ToString()
        {
            return $"[AssetKnowledgeChangedEventArgs: Player={Player}, Delta={string.Join(",", Delta)}]";
        }
    }
}
