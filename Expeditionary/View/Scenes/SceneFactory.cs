using Cardamom.Graphics.Camera;
using Cardamom.Mathematics.Geometry;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using Expeditionary.Controller;
using Expeditionary.Controller.Mapping;
using Expeditionary.Controller.Scenes.Matches;
using Expeditionary.Controller.Scenes.Matches.Layers;
using Expeditionary.Model;
using Expeditionary.View.Mapping;
using Expeditionary.View.Scenes.Galaxies;
using Expeditionary.View.Scenes.Matches;
using Expeditionary.View.Scenes.Matches.Layers;
using OpenTK.Mathematics;

namespace Expeditionary.View.Scenes
{
    public class SceneFactory
    {
        private class NoCollider : ICollider3
        {
            public float? GetRayIntersection(Ray3 ray)
            {
                return null;
            }
        }

        private readonly GalaxyViewFactory _galaxyViewFactory;
        private readonly MapViewFactory _mapViewFactory;
        private readonly FogOfWarLayerFactory _fogOfWarLayerFactory;
        private readonly AssetLayerFactory _assetLayerFactory;
        private readonly HighlightLayerFactory _highlightLayerFactory;

        public SceneFactory(
            GalaxyViewFactory galaxyViewFactory,
            MapViewFactory mapViewFactory,
            FogOfWarLayerFactory fogOfWarLayerFactory,
            AssetLayerFactory assetLayerFactory,
            HighlightLayerFactory highlightLayerFactory)
        {
            _galaxyViewFactory = galaxyViewFactory;
            _mapViewFactory = mapViewFactory;
            _fogOfWarLayerFactory = fogOfWarLayerFactory;
            _assetLayerFactory = assetLayerFactory;
            _highlightLayerFactory = highlightLayerFactory;
        }

        public GalaxyScene CreateGalaxy()
        {
            var camera = new SubjectiveCamera3d(100);
            camera.SetPitch(-MathF.PI / 2);
            camera.SetYaw(MathF.PI / 2);
            camera.SetDistance(1);

            var galaxyController = new NoOpElementController();
            var galaxy = new InteractiveModel(_galaxyViewFactory.Create(), new NoCollider(), galaxyController);

            var scene = 
                new GalaxyScene(
                    new PassthroughController(new Camera2dController(camera)
                    {
                        KeySensitivity = 0.0005f,
                        DistanceRange = new(0.025f, 1),
                        MouseWheelSensitivity = 0.025f
                    }),
                    camera,
                    galaxy);

            galaxy.Parent = scene;

            return scene;
        }

        public MatchScene Create(Match match, TerrainViewParameters parameters, int seed)
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

            var fogOfWarController = new FogOfWarLayerController();
            var fogOfWarLayer = 
                new InteractiveModel(
                    _fogOfWarLayerFactory.Create(match.GetMap()), new NoCollider(), fogOfWarController);

            var assetController = new AssetLayerController(match);
            var assetLayer =
                new InteractiveModel(_assetLayerFactory.Create(), new Plane(new(), Vector3.UnitY), assetController);

            var highlightLayer = _highlightLayerFactory.Create();

            var scene =
                new MatchScene(
                    new MatchSceneController(
                        match, 
                        new Camera2dController(camera)
                        {
                            KeySensitivity = 0.0005f,
                            DistanceRange = new(5, 100),
                            MouseWheelSensitivity = 2
                        }),
                    camera,
                    map,
                    fogOfWarLayer,
                    assetLayer,
                    highlightLayer);

            map.Parent = scene;
            assetLayer.Parent = map;
            fogOfWarLayer.Parent = map;

            return scene;
        }
    }
}
