using Cardamom.Audio;
using Cardamom.Graphics;
using Cardamom.Ui;
using Cardamom.Window;
using Expeditionary.Model;
using Expeditionary.Runners.Loaders;
using Expeditionary.View.Mapping;
using Expeditionary.View.Scenes;
using Expeditionary.View.Scenes.Galaxies;
using Expeditionary.View.Scenes.Matches.Layers;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Expeditionary.Runners
{
    public abstract class UiRunner : IProgramRunner
    {
        private readonly ProgramConfig _config;

        public UiRunner(ProgramConfig config)
        {
            _config = config;
        }

        public void Run()
        {
            var monitor = Monitors.GetPrimaryMonitor();
            var window =
                new RenderWindow(
                    "Expeditionary", new Vector2i(monitor.HorizontalResolution, monitor.VerticalResolution), false);
            var ui = new UiWindow(window);
            ui.Bind(new MouseListener());
            ui.Bind(
                new KeyboardListener(SimpleKeyMapper.Us, new Keys[] { Keys.Left, Keys.Right, Keys.Up, Keys.Down }));

            var data = new ProgramDataLoader(_config).Load();
            var resources = data.Resources;

            var uiElementFactory = new UiElementFactory(new AudioPlayer(),resources);

            var sceneFactory =
                new SceneFactory(
                    new GalaxyViewFactory(resources.GetShader("shader-galaxy")),
                    new MapViewFactory(
                        new()
                        {
                            ElevationGradient = new(0.9f, 1.25f),
                            RidgeShift = new(0f, 0.5f, 0.5f, 1)
                        },
                        data.TextureLibrary,
                        resources.GetShader("shader-filter-no-tex"),
                        resources.GetShader("shader-mask"),
                        resources.GetShader("shader-default")),
                    new FogOfWarLayerFactory(resources.GetShader("shader-default"), data.TextureLibrary.Partitions),
                    new AssetLayerFactory(
                        resources.GetShader("shader-default"),
                        data.UnitTextures),
                    new HighlightLayerFactory(resources.GetShader("shader-default-no-tex")));

            SoundtrackPlayer soundtrack = 
                new(uiElementFactory.GetAudioPlayer(), data.Playlist, SoundtrackPlayer.PlayMode.Shuffle);
            soundtrack.Initialize();


            ui.SetRoot(MakeRoot(data, uiElementFactory, sceneFactory));
            ui.Start();
        }

        protected abstract IRenderable MakeRoot(
            ProgramData data, UiElementFactory uiElementFactory, SceneFactory sceneFactory);
    }
}
