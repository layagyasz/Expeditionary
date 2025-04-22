using Expeditionary.Model.Units;

namespace Expeditionary.Model.Knowledge
{
    public record class AssetKnowledgeChangedEventArgs(Player Player, List<IAsset> Delta)
    {
        public override string ToString()
        {
            return $"[AssetKnowledgeChangedEventArgs: Player={Player}, Delta={string.Join(",", Delta)}]";
        }
    }
}
