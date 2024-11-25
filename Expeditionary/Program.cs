using Cardamom;
using Cardamom.Json;
using Cardamom.Json.Graphics.TexturePacking;
using Cardamom.Json.OpenTK;
using Cardamom.Ui;
using Cardamom.Window;
using Expeditionary.Controller;
using Expeditionary.Hexagons;
using Expeditionary.Model;
using Expeditionary.Model.Knowledge;
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

            var partitionTextureGenerator = new PartitionTextureGenerator(resources.GetShader("shader-partition"));
            var partitions = 
                partitionTextureGenerator.Generate(
                    frequencyRange: new(0.5f, 4f), magnitudeRange: new(0f, 4f), seed: 0, count: 60);

            var maskTextureGenerator = new MaskTextureGenerator(resources.GetShader("shader-mask"));
            maskTextureGenerator.Generate(
                frequencyRange: new(4f, 8f), magnitudeRange: new(0f, 1f), seed: 0, count: 16);

            var riverTextureGenerator = new RiverTextureGenerator(resources.GetShader("shader-river"));
            var edges = riverTextureGenerator.Generate(
                frequencyRange: new(0.5f, 4f), magnitudeRange: new(0f, 2f), seed: 0, count: 10);

            var structureTextureGenerator = 
                new StructureTextureGenerator(resources.GetShader("shader-default-no-tex"));
            var structures = structureTextureGenerator.Generate();

            var sceneFactory =
                new SceneFactory(
                    new MapViewFactory(
                        new()
                        {
                            ElevationGradient = new(0.75f, 2f),
                        },
                        new(partitions, edges, structures),
                        resources.GetShader("shader-filter-no-tex"),
                        resources.GetShader("shader-mask-no-tex"),
                        resources.GetShader("shader-default")),
                    new FogOfWarLayerFactory(resources.GetShader("shader-default"), partitions),
                    new AssetLayerFactory(
                        resources.GetShader("shader-default"),
                        unitTextures),
                    new HighlightLayerFactory(resources.GetShader("shader-default-no-tex")));


            var environmentDefinition = module.Environments["environment-default"];
            var environment = environmentDefinition.GetEnvironment();
            var mapSize = new Vector2i(100, 100);
            var map = MapGenerator.Generate(environment.Parameters, mapSize, seed: new Random().Next());
            var player = new Player(Id: 0, Team: 0, module.Factions["faction-hyacinth"]);
            var opponent = new Player(Id: 1, Team: 1, module.Factions["faction-poticas"]);
            var players = new List<Player>() { player, opponent };
            var match =
                new Match(
                    new SerialIdGenerator(), 
                    map,
                    players.ToDictionary(
                        x => x, 
                        x => new PlayerKnowledge(x, map, new AssetKnowledge(x), new(map, new KnownMapDiscovery()))));
            var driver = new GameDriver(match, players, new());
            driver.Step();

            var center = Cubic.HexagonalOffset.Instance.Wrap(new(50, 50));
            match.Add(module.UnitTypes["def-mg-example"], player, center);
            foreach (var surround in Geometry.GetNeighbors(center).Take(3))
            {
                match.Add(module.UnitTypes.First().Value, opponent, surround);
            }
            match.Initialize();

            var terrainParameters = environment.Appearance.Materialize(sensitivity);
            RecordPalette(terrainParameters);

            ui.SetRoot(
                new MatchScreen(
                    new MatchController(driver, player), 
                    sceneFactory.Create(match, terrainParameters, seed: 0), 
                    new UnitOverlay(uiElementFactory)));
            ui.Start();
        }

        private static void RecordPalette(TerrainViewParameters parameters)
        {
            var palette = new Cardamom.Graphics.Image(7, 3);
            palette.Set(0, 0, parameters.Liquid);
            palette.Set(1, 0, parameters.Stone.A);
            palette.Set(1, 1, parameters.Stone.B);
            palette.Set(1, 2, parameters.Stone.C);
            palette.Set(2, 0, parameters.Soil.A);
            palette.Set(2, 1, parameters.Soil.B);
            palette.Set(2, 2, parameters.Soil.C);
            palette.Set(3, 0, parameters.Brush.TopLeft);
            palette.Set(3, 1, parameters.Brush.BottomLeft);
            palette.Set(4, 0, parameters.Brush.TopRight);
            palette.Set(4, 1, parameters.Brush.BottomRight);
            palette.Set(5, 0, parameters.Foliage.TopLeft);
            palette.Set(5, 1, parameters.Foliage.BottomLeft);
            palette.Set(6, 0, parameters.Foliage.TopRight);
            palette.Set(6, 1, parameters.Foliage.BottomRight);
            palette.SaveToFile("palette.bmp");
        }
    }
}
