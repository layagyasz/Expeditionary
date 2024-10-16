using Cardamom.Graphics.Camera;
using Cardamom.Ui.Controller.Element;
using Expeditionary.Controller.Mapping;
using Expeditionary.Model;
using Expeditionary.Model.Combat;
using Expeditionary.View;
using Expeditionary.View.Scenes.Matches;

namespace Expeditionary.Controller.Scenes.Matches
{
    public class MatchSceneController : SceneController
    {
        public EventHandler<AssetClickedEventArgs>? AssetClicked { get; set; }
        public EventHandler<HexClickedEventArgs>? HexClicked { get; set; }

        private readonly Match _match;
        private readonly ICamera _camera;
        private readonly MapController _mapController;
        private readonly AssetLayerController _assetLayerController;
        private readonly HighlightLayer _highlightLayer;

        private MatchScene? _scene;

        public MatchSceneController(
            Match match,
            Camera2dController cameraController,
            ICamera camera,
            MapController mapController,
            AssetLayerController assetLayerController,
            HighlightLayer highlightLayer)
            : base(cameraController)
        {
            _match = match;
            _camera = camera;
            _mapController = mapController;
            _assetLayerController = assetLayerController;
            _highlightLayer = highlightLayer;
        }

        public override void Bind(object @object)
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

        public override void Unbind()
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
            AssetClicked?.Invoke(this, e);
        }

        private void HandleHexClicked(object? sender, HexClickedEventArgs e)
        {
            HexClicked?.Invoke(this, e);
        }
    }
}
