﻿using Cardamom;
using Cardamom.Json;
using Cardamom.Json.Graphics.TexturePacking;
using Cardamom.Json.OpenTK;
using Cardamom.Mathematics.Color;
using Cardamom.Ui;
using Cardamom.Window;
using Expeditionary.Controller;
using Expeditionary.Hexagons;
using Expeditionary.Model;
using Expeditionary.Model.Mapping.Generator;
using Expeditionary.Spectra;
using Expeditionary.View;
using Expeditionary.View.Mapping;
using Expeditionary.View.Scenes;
using Expeditionary.View.Scenes.Matches;
using Expeditionary.View.Textures.Generation;
using Expeditionary.View.Textures.Generation.Combat.Units;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Expeditionary
{
    public class Program
    {
        public static void Main()
        {
            var window = new RenderWindow("Expeditionary", new(512, 512));
            var ui = new UiWindow(window);
            ui.Bind(new MouseListener());
            ui.Bind(
                new KeyboardListener(SimpleKeyMapper.Us, new Keys[] { Keys.Left, Keys.Right, Keys.Up, Keys.Down }));

            var resources = GameResources.Builder.ReadFrom("resources/view/ui.json").Build();
            var uiElementFactory = new UiElementFactory(resources);

            JsonSerializerOptions options = new()
            {
                NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
                ReadCommentHandling = JsonCommentHandling.Skip,
                ReferenceHandler = new KeyedReferenceHandler()
            };
            options.Converters.Add(new ColorJsonConverter());
            options.Converters.Add(new Vector2iJsonConverter());
            options.Converters.Add(new TextureVolumeJsonConverter());
            var module =
                JsonSerializer.Deserialize<GameModule>(
                    File.ReadAllText("resources/config/default/module.json"), options)!;
            var unitTextureGeneratorSettings =
                JsonSerializer.Deserialize<UnitTextureGeneratorSettings>(
                    File.ReadAllText("resources/view/unit_texture_generator_settings.json"), options)!;
            var unitTextureGenerator = new UnitTextureGenerator(unitTextureGeneratorSettings);
            var unitTextures = unitTextureGenerator.Generate(module.UnitTypes.Values);
            unitTextures.GetTextures().First().CopyToImage().SaveToFile("units.png");

            var sensitivity = 
                JsonSerializer.Deserialize<SpectrumSensitivity>(
                    File.ReadAllText("resources/view/human_eye_sensitivity.json"))!;
            var baseColor =
                Color4.ToHsv(ColorSystem.Ntsc.Transform(sensitivity.GetColor(new BlackbodySpectrum(5772).GetPeak())));

            var partitionTextureGenerator = new PartitionTextureGenerator(resources.GetShader("shader-partition"));
            var partitions = 
                partitionTextureGenerator.Generate(
                    frequencyRange: new(0.5f, 4f), attenuationRange: new(0.5f, 4f), seed: 0, count: 60);

            var maskTextureGenerator = new MaskTextureGenerator(resources.GetShader("shader-mask"));
            maskTextureGenerator.Generate(frequencyRange: new(4f, 8f), seed: 0, count: 16);

            var riverTextureGenerator = new RiverTextureGenerator(resources.GetShader("shader-river"));
            var edges = riverTextureGenerator.Generate(
                frequencyRange: new(0.5f, 4f), attenuationRange: new(0.5f, 2f), seed: 0, count: 10);

            var structureTextureGenerator = 
                new StructureTextureGenerator(resources.GetShader("shader-default-no-tex"));
            var structures = structureTextureGenerator.Generate();

            var mapGenerator = new MapGenerator();
            var sceneFactory =
                new SceneFactory(
                    new MapViewFactory(
                        new()
                        {
                            ElevationGradient = new(0.75f, 2f),
                        },
                        new(partitions, edges, structures),
                        resources.GetShader("shader-mask-no-tex"),
                        resources.GetShader("shader-default")),
                    new AssetLayerFactory(
                        resources.GetShader("shader-default"),
                        unitTextures),
                    new HighlightLayerFactory(resources.GetShader("shader-default-no-tex")));

            var terrainParameters =
                new TerrainViewParameters()
                {
                    Liquid = new(27, 55, 85, 255),
                    Stone = new()
                    {
                        // Gray
                        A = new(200, 200, 200, 255),
                        // White
                        B = new(240, 240, 240, 255),
                        // Granite
                        C = new(227, 173, 156, 255)
                    },
                    Soil = new()
                    {
                        // Sand
                        A = new(248, 240, 133, 255),
                        // Clay
                        B = new(196, 164, 81, 255),
                        // Silt
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

            var environment = module.Environments["environment-default"];
            var mapParameters = environment.GetParameters();
            var map = mapGenerator.Generate(mapParameters, new(100, 100), seed: new Random().Next());
            var match = new Match(new SerialIdGenerator(), map);
            var player = new Player(Id: 0, Team: 0, module.Factions["faction-hyacinth"]);
            var opponent = new Player(Id: 1, Team: 1, module.Factions["faction-poticas"]);
            var driver = new GameDriver(match, new List<Player>() { player, opponent }, new());
            driver.Step();

            var center = Cubic.HexagonalOffset.Instance.Wrap(new(50, 50));
            match.Add(module.UnitTypes["def-mg-example"], player, center);
            foreach (var surround in Geometry.GetNeighbors(center))
            {
                match.Add(module.UnitTypes.First().Value, opponent, surround);
            }

            ui.SetRoot(
                new MatchScreen(
                    new MatchController(driver, player), 
                    sceneFactory.Create(match, terrainParameters, seed: 0), 
                    new UnitOverlay(uiElementFactory)));
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
