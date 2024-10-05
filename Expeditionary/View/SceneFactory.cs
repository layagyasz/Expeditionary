using Cardamom.Graphics.Camera;
using Cardamom.Mathematics.Geometry;
using Cardamom.Ui;
using Cardamom.Ui.Elements;
using Expeditionary.Controller;
using Expeditionary.Model;
using OpenTK.Mathematics;

namespace Expeditionary.View
{
    public class SceneFactory
    {
        private readonly MapViewFactory _mapViewFactory;
        private readonly AssetLayerFactory _assetLayerFactory;

        public SceneFactory(MapViewFactory mapViewFactory, AssetLayerFactory assetLayerFactory)
        {
            _mapViewFactory = mapViewFactory;
            _assetLayerFactory = assetLayerFactory;
        }

        public IScene Create(Match match, TerrainViewParameters parameters, int seed)
        {
            var camera = new SubjectiveCamera3d(100);
            camera.SetPitch(-MathF.PI / 2);
            camera.SetYaw(MathF.PI / 2);
            camera.SetDistance(20);

            var mapController = new MapController();
            var map = 
                new InteractiveModel(
                    _mapViewFactory.Create(match.GetMap(), parameters, seed),
                    new Plane(new(), Vector3.UnitY),
                    mapController);

            var assetController = new AssetLayerController();
            var assetLayer =
                new InteractiveModel(_assetLayerFactory.Create(), new Plane(new(), Vector3.UnitY), assetController);

            var scene =
                new MatchScene(
                    new SceneController(
                        new Camera2dController(camera)
                        {
                            KeySensitivity = 0.0005f,
                            DistanceRange = new(5, 100),
                            MouseWheelSensitivity = 2
                        }, 
                        new MatchSceneController(match, camera, mapController, assetController)),
                    camera,
                    map,
                    assetLayer);

            map.Parent = scene;
            assetLayer.Parent = map;

            return scene;
        }
    }
}
