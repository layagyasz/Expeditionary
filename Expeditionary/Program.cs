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

            var partitionTextureGenerator = 
                new PartitionTextureGenerator(
                    new RenderShader.Builder()
                        .SetVertex("Resources/View/Textures/Generation/default.vert")
                        .SetFragment("Resources/View/Textures/Generation/partition.frag")
                        .Build());
            var partitions = 
                partitionTextureGenerator.Generate(
                    frequencyRange: new(0.5f, 4f), attenuationRange: new(0.5f, 4f), seed: 0, count: 60);

            var maskTextureGenerator =
                new MaskTextureGenerator(
                    new RenderShader.Builder()
                        .SetVertex("Resources/View/Textures/Generation/default.vert")
                        .SetFragment("Resources/View/Textures/Generation/mask.frag")
                        .Build());
            maskTextureGenerator.Generate(frequencyRange: new(4f, 8f), seed: 0, count: 16);

            var riverTextureGenerator = 
                new RiverTextureGenerator(new RenderShader.Builder()
                    .SetVertex("Resources/View/Textures/Generation/default.vert")
                    .SetFragment("Resources/View/Textures/Generation/river.frag")
                    .Build());
            var edges = riverTextureGenerator.Generate(
                frequencyRange: new(0.5f, 4f), attenuationRange: new(0.5f, 2f), seed: 0, count: 10);

            var mapGenerator = new MapGenerator();
            var sceneFactory = 
                new SceneFactory(
                    new MapViewFactory(
                        new()
                        {
                            ElevationGradient = new(0.8f, 1.2f),
                            ElevationLevel = 5
                        },
                        new(partitions, edges),
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
                    Stone = new()
                    {
                        A = new(200, 200, 200, 255),
                        B = new(240, 240, 240, 255),
                        C = new(227, 173, 156, 255)
                    },
                    Soil = new()
                    {
                        A = new(248, 240, 133, 255),
                        B = new(196, 164, 81, 255),
                        C = new(59, 48, 45, 255)
                    },
                    Brush = new()
                    {
                        // Hot & Dry
                        TopRight = Color4.FromHsv(CombineHsv(baseColor, new(-0.33f, 0.25f, 0.9f, 1f))),
                        // Hot & Wet
                        BottomRight = Color4.FromHsv(CombineHsv(baseColor, new(-0.13f, 0.67f, 0.4f, 1f))),
                        // Cold & Dry
                        TopLeft = Color4.FromHsv(CombineHsv(baseColor, new(-0.34f, 0.32f, 0.7f, 1f))),
                        // Cold & Wet
                        BottomLeft = Color4.FromHsv(CombineHsv(baseColor, new(0.03f, 0.2f, 0.7f, 1f))),
                    },
                    Foliage = new()
                    {
                        // Hot & Dry
                        TopRight = Color4.FromHsv(CombineHsv(baseColor, new(-0.32f, 0.45f, 0.5f, 1f))),
                        // Hot & Wet
                        BottomRight = Color4.FromHsv(CombineHsv(baseColor, new(-0.02f, 0.45f, 0.17f, 1f))),
                        // Cold & Dry
                        TopLeft = Color4.FromHsv(CombineHsv(baseColor, new(-0.08f, 0.64f, 0.5f, 1f))),
                        // Cold & Wet
                        BottomLeft = Color4.FromHsv(CombineHsv(baseColor, new(0.03f, 0.8f, 0.3f, 1f))),
                    }
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
            var palette = new Cardamom.Graphics.Image(8, 3);
            palette.Set(0, 0, baseColor);
            palette.Set(1, 0, parameters.Liquid);
            palette.Set(2, 0, parameters.Stone.A);
            palette.Set(2, 1, parameters.Stone.B);
            palette.Set(2, 2, parameters.Stone.C);
            palette.Set(3, 0, parameters.Soil.A);
            palette.Set(3, 1, parameters.Soil.B);
            palette.Set(3, 2, parameters.Soil.C);
            palette.Set(4, 0, parameters.Brush.TopLeft);
            palette.Set(4, 1, parameters.Brush.BottomLeft);
            palette.Set(5, 0, parameters.Brush.TopRight);
            palette.Set(5, 1, parameters.Brush.BottomRight);
            palette.Set(6, 0, parameters.Foliage.TopLeft);
            palette.Set(6, 1, parameters.Foliage.BottomLeft);
            palette.Set(7, 0, parameters.Foliage.TopRight);
            palette.Set(7, 1, parameters.Foliage.BottomRight);
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
