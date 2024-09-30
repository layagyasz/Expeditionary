using Cardamom.Graphics.Camera;
using Cardamom.Ui;
using Expeditionary.Controller;
using Expeditionary.Model;

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
            return new MatchScene(
                new SceneController(
                    new Camera2dController(camera)
                    {
                        KeySensitivity = 0.0005f,
                        DistanceRange = new(5, 100),
                        MouseWheelSensitivity = 2
                    }, 
                    new MatchSceneController(match)),
                camera,
                _mapViewFactory.Create(match.GetMap(), parameters, seed),
                _assetLayerFactory.Create());
        }
    }
}
