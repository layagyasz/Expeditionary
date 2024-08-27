using Cardamom.Graphics;
using Cardamom.Mathematics.Color;
using Cardamom.Ui;
using Cardamom.Window;
using Expeditionary.Model.Mapping;
using Expeditionary.Spectra;
using Expeditionary.View;
using Expeditionary.View.Textures.Generation;
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
                Color4.ToHsv(ColorSystem.Ntsc.Transform(sensitivity.GetColor(new BlackbodySpectrum(5772).GetPeak())));

            var terrainTextureGenerator = 
                new TerrainTextureGenerator(
                    new RenderShader.Builder()
                        .SetVertex("Resources/View/Textures/Generation/default.vert")
                        .SetFragment("Resources/View/Textures/Generation/partition.frag").Build());
            var terrains = 
                terrainTextureGenerator.Generate(
                    frequencyRange: new(0.5f, 4f), attenuationRange: new(0.5f, 4f), seed: 0, count: 60);

            var riverTextureGenerator = 
                new RiverTextureGenerator(new RenderShader.Builder()
                    .SetVertex("Resources/View/Textures/Generation/default.vert")
                    .SetFragment("Resources/View/Textures/Generation/river.frag").Build());
            var edges = riverTextureGenerator.Generate(
                frequencyRange: new(0.5f, 4f), attenuationRange: new(0.5f, 2f), seed: 0, count: 20);

            var mapGenerator = new MapGenerator();
            var sceneFactory = 
                new SceneFactory(
                    new MapViewFactory(
                        new()
                        {
                            ElevationGradient = new(0.8f, 1.2f),
                            ElevationLevel = 5
                        },
                        new(edges, terrains),
                        new RenderShader.Builder()
                            .SetVertex("Resources/View/default.vert")
                            .SetFragment("Resources/View/mask_no_tex.frag")
                            .Build(),
                        new RenderShader.Builder()
                            .SetVertex("Resources/View/default.vert")
                            .SetFragment("Resources/View/default.frag")
                            .Build()));

            var terrainParameters =
                new TerrainViewParameters()
                {
                    Liquid = new(27, 55, 85, 255),
                    Stone0 = new(200, 200, 200, 255),
                    Stone1 = new(240, 240, 240, 255),
                    Stone2 = new(227, 173, 156, 255),
                    Sand = new(248, 240, 133, 255),
                    Clay = new(196, 164, 81, 255),
                    Silt = new(59, 48, 45, 255),
                    HotDry = Color4.FromHsv(CombineHsv(baseColor, new(-0.33f, 0.25f, 0.9f, 1f))),
                    HotWet = Color4.FromHsv(CombineHsv(baseColor, new(-0.13f, 0.67f, 0.4f, 1f))),
                    ColdDry = Color4.FromHsv(CombineHsv(baseColor, new(-0.34f, 0.32f, 0.7f, 1f))),
                    ColdWet = Color4.FromHsv(CombineHsv(baseColor, new(0.03f, 0.2f, 0.7f, 1f))),
                };
            RecordPalette(Color4.FromHsv(baseColor), terrainParameters);
            ui.SetRoot(
                sceneFactory.Create(
                    mapGenerator.Generate(
                        new(),
                        new(100, 100),
                        seed: new Random().Next()),
                    terrainParameters,
                    seed: 0));
            ui.Start();
        }

        private static void RecordPalette(Color4 baseColor, TerrainViewParameters parameters)
        {
            var palette = new Cardamom.Graphics.Image(6, 3);
            palette.Set(0, 0, baseColor);
            palette.Set(1, 0, parameters.Liquid);
            palette.Set(2, 0, parameters.Stone0);
            palette.Set(2, 1, parameters.Stone1);
            palette.Set(2, 2, parameters.Stone2);
            palette.Set(3, 0, parameters.Sand);
            palette.Set(3, 1, parameters.Clay);
            palette.Set(3, 2, parameters.Silt);
            palette.Set(4, 0, parameters.HotDry);
            palette.Set(4, 1, parameters.HotWet);
            palette.Set(5, 0, parameters.ColdDry);
            palette.Set(5, 1, parameters.ColdWet);
            palette.SaveToFile("palette.bmp");
        }

        private static Vector4 CombineHsv(Vector4 color, Vector4 adjustment)
        {
            var hsv = color;
            hsv.X = (hsv.X + 1 + adjustment.X) % 1;
            hsv.Y = MathHelper.Clamp(hsv.Y * adjustment.Y, 0, 1);
            hsv.Z = MathHelper.Clamp(hsv.Z * adjustment.Z, 0, 1);
            return hsv;
        }
    }
}
