using Cardamom.Graphics;
using Cardamom.Mathematics.Color;
using Cardamom.Ui;
using Cardamom.Window;
using Expeditionary.Generation;
using Expeditionary.Model.Mapping;
using Expeditionary.Spectra;
using Expeditionary.View;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Text.Json;

namespace Expeditionary
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var window = new RenderWindow("Expeditionary", new(512, 512));
            var ui = new UiWindow(window);
            ui.Bind(new MouseListener());
            ui.Bind(
                new KeyboardListener(SimpleKeyMapper.Us, new Keys[] { Keys.Left, Keys.Right, Keys.Up, Keys.Down }));

            var sensitivity = 
                JsonSerializer.Deserialize<SpectrumSensitivity>(
                    File.ReadAllText("Resources/View/HumanEyeSensitivity.json"))!;
            var baseColor =
                Color4.ToHsl(ColorSystem.Ntsc.Transform(sensitivity.GetColor(new BlackbodySpectrum(4500).GetPeak())));

            var tileBaseGenerator = 
                new TerrainTextureGenerator(
                    new RenderShader.Builder()
                        .SetVertex("Resources/Generation/default.vert")
                        .SetFragment("Resources/Generation/partition.frag").Build());
            var tileBases = tileBaseGenerator.Generate(attenuationRange: new(0.5f, 4f), seed: 0, count: 60);

            var mapGenerator = new MapGenerator();
            var sceneFactory = 
                new SceneFactory(
                    new MapViewFactory(
                        new()
                        {
                            ElevationGradient = new(0.8f, 1.2f),
                            ElevationLevel = 5
                        },
                        tileBases,
                        new RenderShader.Builder()
                            .SetVertex("Resources/View/default.vert")
                            .SetFragment("Resources/View/default.frag")
                            .Build()));

            ui.SetRoot(
                sceneFactory.Create(
                    new(512, 512, 0), 
                    mapGenerator.Generate(
                        new(),
                        new(100, 100),
                        seed: new Random().Next()),
                    new() 
                    { 
                        Liquid = new(27, 55, 85, 255),
                        Stone0 = new(200, 200, 200, 255),
                        Stone1 = new(240, 240, 240, 255),
                        Stone2 = new(227, 173, 156, 255),
                        Sand = new(248, 240, 133, 255),
                        Clay = new(196, 164, 81, 255),
                        Silt = new(59, 48, 45, 255),
                        SolarTemperature = 5772,
                        HotDry = Color4.FromHsl(CombineHsl(baseColor, new(-0.27f, 2f, 1.4f, 1f))),
                        HotWet = Color4.FromHsl(CombineHsl(baseColor, new(-0.07f, 1.3f, 0.9f, 1f))),
                        ColdDry = Color4.FromHsl(CombineHsl(baseColor, new(-0.34f, 0.32f, 0.7f, 1f))),
                        ColdWet = Color4.FromHsl(CombineHsl(baseColor, new(0.03f, 0.2f, 0.7f, 1f))),
                    },
                    seed: 0));
            ui.Start();
        }

        private static Vector4 CombineHsl(Vector4 color, Vector4 adjustment)
        {
            var hsl = color;
            hsl.X = (hsl.X + 1 + adjustment.X) % 1;
            hsl.Y *= adjustment.Y;
            hsl.Z *= adjustment.Z;
            return hsl;
        }
    }
}
