using Cardamom.Audio;
using Cardamom.Graphics;
using Cardamom.Json;
using Cardamom.Ui;
using Cardamom.Window;
using Expeditionary.Runners.Loaders;
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
            AudioPlayer audioPlayer = new();
            SoundtrackPlayer soundtrack = new(audioPlayer, data.Playlist, SoundtrackPlayer.PlayMode.Shuffle);
            soundtrack.Initialize();
            ui.SetRoot(MakeRoot(data));
            ui.Start();
        }

        protected abstract IRenderable MakeRoot(ProgramData data);
    }
}
