using Cardamom.Ui.Controller;
using Expeditionary.Model;
using Expeditionary.Model.Combat;
using Expeditionary.View;

namespace Expeditionary.Controller
{
    public class MatchSceneController : IController
    {
        private readonly Match _match;

        private MatchScene? _scene;
        
        public MatchSceneController(Match match)
        {
            _match = match;
        }

        public void Bind(object @object)
        {
            _scene = @object as MatchScene;

            _match.AssetAdded += HandleAssetAdded;
            _match.AssetRemoved += HandleAssetRemoved;

            foreach (var asset in _match.GetAssets())
            {
                _scene!.AddAsset(asset);
            }
        }

        public void Unbind()
        {
            _scene = null;

            _match.AssetAdded -= HandleAssetAdded;
            _match.AssetRemoved -= HandleAssetRemoved;
        }

        private void HandleAssetAdded(object? sender, IAsset asset)
        {
            _scene?.AddAsset(asset);
        }

        private void HandleAssetRemoved(object? sender, IAsset asset)
        {
            _scene?.RemoveAsset(asset);
        }
    }
}
