using Cardamom.Graphics.Camera;
using Cardamom.Ui;
using Expeditionary.Controller;
using Expeditionary.Model.Mapping;

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

        public IScene Create(Map map, TerrainViewParameters parameters, int seed)
        {
            var camera = new SubjectiveCamera3d(100);
            camera.SetPitch(-MathF.PI / 2);
            camera.SetYaw(MathF.PI / 2);
            camera.SetDistance(20);
            return new MatchScene(
                new Camera2dController(camera)
                {
                    KeySensitivity = 0.0005f,
                    DistanceRange = new(5, 100),
                    MouseWheelSensitivity = 2
                },
                camera,
                _mapViewFactory.Create(map, parameters, seed),
                _assetLayerFactory.Create());
        }
    }
}
