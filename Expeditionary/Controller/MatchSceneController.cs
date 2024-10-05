﻿using Cardamom.Graphics.Camera;
using Cardamom.Ui.Controller;
using Expeditionary.Model;
using Expeditionary.Model.Combat;
using Expeditionary.View;

namespace Expeditionary.Controller
{
    public class MatchSceneController : IController
    {
        private readonly Match _match;
        private readonly ICamera _camera;
        private readonly MapController _mapController;
        private readonly AssetLayerController _assetLayerController;

        private MatchScene? _scene;
        
        public MatchSceneController(
            Match match, ICamera camera, MapController mapController, AssetLayerController assetLayerController)
        {
            _match = match;
            _camera = camera;
            _mapController = mapController;
            _assetLayerController = assetLayerController;
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
        }

        public void Unbind()
        {
            _scene = null;

            _match.AssetAdded -= HandleAssetAdded;
            _match.AssetRemoved -= HandleAssetRemoved;

            _camera.Changed -= HandleCameraChanged;
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
    }
}
