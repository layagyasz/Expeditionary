using Expeditionary.Model.Combat;

namespace Expeditionary.Model.Knowledge
{
    public class AssetKnowledge
    {
        private readonly Dictionary<IAsset, SingleAssetKnowledge> _assets = new();

        public SingleAssetKnowledge Get(IAsset asset)
        {
            return _assets.TryGetValue(asset, out var value) ? value : new(false, null);
        }
    }
}
