﻿using Cardamom;
using Cardamom.Json;
using Cardamom.Json.Graphics.TexturePacking;
using Cardamom.Json.OpenTK;
using Cardamom.Ui;
using Cardamom.Window;
using Expeditionary.Controller;
using Expeditionary.Hexagons;
using Expeditionary.Model;
using Expeditionary.Model.Factions;
using Expeditionary.Model.Formations;
using Expeditionary.Model.Mapping.Generator;
using Expeditionary.Model.Missions;
using Expeditionary.Model.Missions.Deployments;
using Expeditionary.Model.Missions.Objectives;
using Expeditionary.Model.Units;
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
            options.Converters.Add(new BuilderJsonConverter());
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

            var partitionTextureGenerator =
                new PartitionTextureGenerator(resources.GetShader("shader-generate-partition"));
            var partitions =
                partitionTextureGenerator.Generate(
                    frequencyRange: new(0.5f, 4f), magnitudeRange: new(0f, 4f), seed: 0, count: 60);

            var maskTextureGenerator = new MaskTextureGenerator(resources.GetShader("shader-generate-mask"));
            var masks = maskTextureGenerator.Generate(
                frequencyRange: new(4f, 8f), magnitudeRange: new(0f, 1f), seed: 0, count: 16);

            var riverTextureGenerator = new RiverTextureGenerator(resources.GetShader("shader-generate-river"));
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
                        new(edges, masks, partitions, structures),
                        resources.GetShader("shader-filter-no-tex"),
                        resources.GetShader("shader-mask"),
                        resources.GetShader("shader-default")),
                    new FogOfWarLayerFactory(resources.GetShader("shader-default"), partitions),
                    new AssetLayerFactory(
                        resources.GetShader("shader-default"),
                        unitTextures),
                    new HighlightLayerFactory(resources.GetShader("shader-default-no-tex")));

            var random = new Random();
            var environmentDefinition = module.Environments.ToList()[random.Next(module.Environments.Count)].Value;
            Console.WriteLine(environmentDefinition.Key);

            var player = new Player(Id: 0, Team: 0, module.Factions["faction-hyacinth"]);
            var opponent = new Player(Id: 1, Team: 1, module.Factions["faction-poticas"]);
            var mission = 
                new Mission(
                    new MapSetup(
                        environmentDefinition,
                        new Vector2i(100, 100), 
                        Enumerable.Empty<CityGenerator.LayerParameters>()),
                    new() 
                    { 
                        new(
                            player, 
                            new(Enumerable.Empty<IObjective>()), 
                            new() 
                            { 
                                new(RandomFormation(module, player.Faction, random), new RandomDeployment()) 
                            }),
                        new(
                            opponent,
                            new(Enumerable.Empty<IObjective>()),
                            new()
                            {
                                new(RandomFormation(module, opponent.Faction, random), new RandomDeployment())
                            })
                    });
            (var match, var appearance) = mission.Setup(new SetupContext(random, new SerialIdGenerator()));
            match.Step();
            match.Initialize();

            var terrainParameters = appearance.Materialize(sensitivity);
            RecordPalette(terrainParameters);
            RecordUnitTypes(module.UnitTypes.Values);

            ui.SetRoot(
                new MatchScreen(
                    new MatchController(match, player),
                    sceneFactory.Create(match, terrainParameters, seed: 0),
                    new UnitOverlay(uiElementFactory)));
            ui.Start();
        }

        private static  FormationTemplate RandomFormation(GameModule module, Faction faction, Random random)
        {
            var formationConfig = module.FactionFormations.Where(x => x.Value.Faction == faction.Key).First().Value;
            var formationGenerator = formationConfig.Formations[random.Next(formationConfig.Formations.Count)];
            return formationGenerator.Generate(new(random, new(), new(), formationConfig.Units));
        }

        private static void RecordUnitTypes(IEnumerable<UnitType> unitTypes)
        {
            using var stream = File.OpenWrite("units.json");
            var options = 
                new JsonSerializerOptions() 
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = true
                };
            JsonSerializer.Serialize(stream, unitTypes, typeof(IEnumerable<UnitType>), options);
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
