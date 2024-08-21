using Cardamom.Graphics;
using Cardamom.Graphics.Camera;
using Cardamom.Ui;
using Expeditionary.Controller;
using Expeditionary.Model.Mapping;
using OpenTK.Mathematics;

namespace Expeditionary.View
{
    public class SceneFactory
    {
        private readonly MapViewFactory _mapViewFactory;

        public SceneFactory(MapViewFactory mapViewFactory)
        {
            _mapViewFactory = mapViewFactory;
        }

        public IScene Create(Vector3 size, Map map, TerrainViewParameters parameters, int seed)
        {
            var camera = new SubjectiveCamera3d(100);
            camera.SetPitch(-MathF.PI / 2);
            camera.SetYaw(MathF.PI / 2);
            camera.SetDistance(20);
            return new BasicScene(
                size,
                new Camera2dController(camera)
                {
                    KeySensitivity = 0.0005f,
                    DistanceRange = new(5, 100),
                    MouseWheelSensitivity = 2
                },
                camera,
                new List<IRenderable>() { _mapViewFactory.Create(map, parameters, seed) });
        }
    }
}
