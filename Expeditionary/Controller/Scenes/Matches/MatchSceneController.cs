using Cardamom.Graphics.Camera;
using Expeditionary.Controller.Mapping;
using Expeditionary.Model;
using Expeditionary.Model.Knowledge;
using Expeditionary.View.Scenes.Matches;

namespace Expeditionary.Controller.Scenes.Matches
{
    public class MatchSceneController : SceneController
    {
        public EventHandler<AssetClickedEventArgs>? AssetClicked { get; set; }
        public EventHandler<HexClickedEventArgs>? HexClicked { get; set; }

        private readonly Match _match;

        private ICamera? _camera;
        private MapController? _mapController;
        private FogOfWarLayerController? _fogOfWarLayerController;
        private AssetLayerController? _assetLayerController;

        private Player? _player;
        private MatchScene? _scene;

        public MatchSceneController(
            Match match,
            Camera2dController cameraController)
            : base(cameraController)
        {
            _match = match;
        }

        public override void Bind(object @object)
        {
            _scene = @object as MatchScene;

            _camera = _scene!.Camera;
            _mapController = _scene!.Map.Controller as MapController;
            _fogOfWarLayerController = _scene!.FogOfWar.Controller as FogOfWarLayerController;
            _assetLayerController = _scene!.Assets.Controller as AssetLayerController;

            _match.AssetKnowledgeChanged += HandleAssetKnowledgeChanged;
            _match.MapKnowledgeChanged += HandleMapKnowledgeChanged;

            _camera.Changed += HandleCameraChanged;

            _assetLayerController!.AssetClicked += HandleAssetClicked;
            _mapController!.HexClicked += HandleHexClicked;
        }

        public override void Unbind()
        {
            _scene = null;

            _match.AssetKnowledgeChanged -= HandleAssetKnowledgeChanged;
            _match.MapKnowledgeChanged -= HandleMapKnowledgeChanged;

            _camera!.Changed -= HandleCameraChanged;
            _camera = null;

            _assetLayerController!.AssetClicked -= HandleAssetClicked;
            _assetLayerController = null;

            _mapController!.HexClicked -= HandleHexClicked;
            _mapController = null;
        }

        public void SetPlayer(Player player)
        {
            _player = player;
            _assetLayerController!.SetKnowledge(_match.GetKnowledge(player));
            _fogOfWarLayerController!.SetKnowledge(_match.GetKnowledge(player));
        }

        private void HandleCameraChanged(object? sender, EventArgs e)
        {
            _mapController!.UpdateGridAlpha(_camera!.Position.Y);
        }

        private void HandleAssetClicked(object? sender, AssetClickedEventArgs e)
        {
            AssetClicked?.Invoke(this, e);
        }

        private void HandleHexClicked(object? sender, HexClickedEventArgs e)
        {
            HexClicked?.Invoke(this, e);
        }

        private void HandleAssetKnowledgeChanged(object? sender, AssetKnowledgeChangedEventArgs e)
        {
            if (e.Player == _player)
            {
                _assetLayerController!.UpdateKnowledge(e.Delta);
            }
        }

        private void HandleMapKnowledgeChanged(object? sender, MapKnowledgeChangedEventArgs e)
        {
            if (e.Player == _player)
            {
                _fogOfWarLayerController!.UpdateKnowledge(_match.GetKnowledge(_player), e.Delta);
            }
        }
    }
}
