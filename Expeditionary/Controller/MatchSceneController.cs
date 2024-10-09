using Cardamom.Graphics.Camera;
using Cardamom.Ui.Controller;
using Expeditionary.Model;
using Expeditionary.Model.Combat;
using Expeditionary.Model.Combat.Units;
using Expeditionary.View;

namespace Expeditionary.Controller
{
    public class MatchSceneController : IController
    {
        private readonly Match _match;
        private readonly ICamera _camera;
        private readonly MapController _mapController;
        private readonly AssetLayerController _assetLayerController;
        private readonly HighlightLayer _highlightLayer;

        private MatchScene? _scene;
        
        public MatchSceneController(
            Match match, 
            ICamera camera,
            MapController mapController,
            AssetLayerController assetLayerController, 
            HighlightLayer highlightLayer)
        {
            _match = match;
            _camera = camera;
            _mapController = mapController;
            _assetLayerController = assetLayerController;
            _highlightLayer = highlightLayer;
        }

        public void Bind(object @object)
        {
            _scene = @object as MatchScene;

            _match.AssetAdded += HandleAssetAdded;
            _match.AssetRemoved += HandleAssetRemoved;

            foreach (var asset in _match.GetAssets())
            {
                _assetLayerController.AddAsset(asset);
            }

            _camera.Changed += HandleCameraChanged;

            _assetLayerController.AssetClicked += HandleAssetClicked;
            _mapController.HexClicked += HandleHexClicked;
        }

        public void Unbind()
        {
            _scene = null;

            _match.AssetAdded -= HandleAssetAdded;
            _match.AssetRemoved -= HandleAssetRemoved;

            _camera.Changed -= HandleCameraChanged;

            _assetLayerController.AssetClicked -= HandleAssetClicked;
            _mapController.HexClicked -= HandleHexClicked;
        }

        private void HandleAssetAdded(object? sender, IAsset asset)
        {
            _assetLayerController.AddAsset(asset);
        }

        private void HandleAssetRemoved(object? sender, IAsset asset)
        {
            _assetLayerController.RemoveAsset(asset);
        }

        private void HandleCameraChanged(object? sender, EventArgs e)
        {
            _mapController.UpdateGridAlpha(_camera.Position.Y);
        }

        private void HandleAssetClicked(object? sender, AssetClickedEventArgs e)
        {
            var asset = e.Assets.First();
            if (asset is Unit unit)
            {
                var range = (int)unit.Type.Attack.First().Range.GetValue();
                _highlightLayer.SetHighlight(
                    Sighting.GetSightField(_match.GetMap(), unit.Position, range)
                        .Select(x => new HighlightLayer.HexHighlight(
                            x.Target, HighlightLayer.GetLevel(x.Distance, new(0, range)))));
            }
        }

        private void HandleHexClicked(object? sender, HexClickedEventArgs e)
        {
            if (_match.GetMap().GetTile(e.Hex) != null)
            {
                var range = 10;
                _highlightLayer.SetHighlight(
                    Sighting.GetSightField(_match.GetMap(), e.Hex, range)
                        .Select(x => new HighlightLayer.HexHighlight(
                            x.Target, HighlightLayer.GetLevel(x.Distance, new(0, range)))));
            }
        }
    }
}
