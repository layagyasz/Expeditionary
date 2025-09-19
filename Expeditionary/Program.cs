using Cardamom;
using Cardamom.Json;
using Cardamom.Json.Graphics.TexturePacking;
using Cardamom.Json.OpenTK;
using Cardamom.Ui;
using Cardamom.Utils.Generators.Samplers;
using Cardamom.Window;
using Expeditionary.Ai;
using Expeditionary.Controller;
using Expeditionary.Model;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Missions;
using Expeditionary.Model.Missions.MissionNodes;
using Expeditionary.Model.Missions.MissionTypes;
using Expeditionary.Spectra;
using Expeditionary.View;
using Expeditionary.View.Common;
using Expeditionary.View.Mapping;
using Expeditionary.View.Scenes;
using Expeditionary.View.Scenes.Matches;
using Expeditionary.View.Textures.Generation;
using Expeditionary.View.Textures.Generation.Combat.Units;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Collections.Immutable;
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
            UnitTypeRecorder.Record("units", module);
            BalanceRecorder.Record("balance", module);

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

            var edgeTextureGenerator = new EdgeTextureGenerator(resources.GetShader("shader-generate-edge"));
            var rivers = edgeTextureGenerator.Generate(
                frequencyRange: new(0.5f, 4f), magnitudeRange: new(0f, 2f), gauge: 9, seed: 0, count: 10);
            var ridges = edgeTextureGenerator.Generate(
                frequencyRange: new(0.5f, 2f), magnitudeRange: new(0f, 1f), gauge: 4, seed: 0, count: 10);

            var structureTextureGenerator =
                new StructureTextureGenerator(resources.GetShader("shader-default-no-tex"));
            var structures = structureTextureGenerator.Generate();

            var sceneFactory =
                new SceneFactory(
                    new MapViewFactory(
                        new()
                        {
                            ElevationGradient = new(0.9f, 1.25f),
                            RidgeShift = new(0f, 0.5f, 0.5f, 1)
                        },
                        new(rivers, ridges, masks, partitions, structures),
                        resources.GetShader("shader-filter-no-tex"),
                        resources.GetShader("shader-mask"),
                        resources.GetShader("shader-default")),
                    new FogOfWarLayerFactory(resources.GetShader("shader-default"), partitions),
                    new AssetLayerFactory(
                        resources.GetShader("shader-default"),
                        unitTextures),
                    new HighlightLayerFactory(resources.GetShader("shader-default-no-tex")));

            var random = new Random();
            var missionResources = 
                new MissionGenerationResources(
                    module.MapEnvironmentGenerator, new(module.FactionFormations, module.Formations), random);
            var missionNode =
                new AssaultMissionNode()
                {
                    Environment = new RandomEnvironmentProvider()
                    {
                        SectorNaming = module.SectorNamings["sector-naming-default"],
                        Sectors = ImmutableList.Create(1, 2, 3, 4, 5)
                    },
                    Difficulty = new() { MissionDifficulty.Medium },
                    Scale = new() { MissionScale.Medium },
                    Attackers = new() { module.Factions["faction-sm"] },
                    Defenders = new() { module.Factions["faction-earth"] },
                    ZoneOptions =
                        new()
                        {
                            new()
                            {
                                CoreCount = 1,
                                CandidateDensity = .005f,
                                Type = StructureType.Mining,
                                Level = 1,
                                Size = new NormalSampler(2, 1),
                                RiverPenalty = new(),
                                CoastPenalty = new(),
                                SlopePenalty = new(0, -1, 1),
                                ElevationPenalty = new(0, -1, 1),
                            }
                        }
                };
            var mission = missionNode.Create(missionResources);
            Console.WriteLine($"{mission.Map.Environment.Key} {mission.Map.Environment.Name}");
            var player = mission.Players.First().Player;
            var creationContext = new CreationContext(player, IsTest: true);
            (var match, var appearance) = mission.Create(random, creationContext);
            var aiManager = new AiManager(match, mission.Players.Select(x => x.Player).Where(x => x != player));
            var setupContext = new SetupContext(random, new SerialIdGenerator(), aiManager);
            mission.Setup(match, setupContext);
            match.Initialize();
            aiManager.Initialize();

            var terrainParameters = appearance.Materialize(sensitivity);

            ui.SetRoot(
                new MatchScreen(
                    new MatchController(match, player),
                    sceneFactory.Create(match, terrainParameters, seed: 0),
                    new UnitOverlay(uiElementFactory),
                    RightClickMenu.Create(uiElementFactory)));
            match.Step();
            ui.Start();
        }
    }
}
