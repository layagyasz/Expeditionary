using Cardamom.Ui;
using Expeditionary.Ai;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Missions.MissionNodes;
using Expeditionary.Model.Missions.MissionTypes;
using Expeditionary.Model.Missions;
using Expeditionary.Model;
using Expeditionary.View.Mapping;
using Expeditionary.View.Scenes.Matches;
using Expeditionary.View.Scenes;
using System.Collections.Immutable;
using Cardamom.Utils.Generators.Samplers;
using Expeditionary.View.Screens;
using Expeditionary.Controller.Screens;
using Expeditionary.View.Scenes.Matches.Layers;
using Expeditionary.View.Common.Components;
using Cardamom.Graphics;
using Cardamom.Audio;

namespace Expeditionary.Runners
{
    public class RandomMissionRunner : UiRunner
    {
        public RandomMissionRunner(ProgramConfig config)
            : base(config) { }

        protected override IRenderable MakeRoot(ProgramData data)
        {
            var resources = data.Resources;
            var uiElementFactory = new UiElementFactory(new AudioPlayer(), resources);

            var sceneFactory =
                new SceneFactory(
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

            var module = data.Module;
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
            foreach (var trait in mission.Map.Environment.Traits)
            {
                Console.WriteLine(trait.Key);
            }
            var player = mission.Players.First().Player;
            var creationContext = new CreationContext(player, IsTest: true);
            (var match, var appearance) = mission.Create(random, creationContext);
            var aiManager = new AiManager(match, mission.Players.Select(x => x.Player).Where(x => x != player));
            var setupContext = new SetupContext(random, new SerialIdGenerator(), aiManager);
            mission.Setup(match, setupContext);
            match.Initialize();
            aiManager.Initialize();

            var terrainParameters = appearance.Materialize(data.SpectrumSensitivity);

            match.Step();
            return new MatchScreen(
                new MatchController(match, player),
                sceneFactory.Create(match, terrainParameters, seed: 0),
                new UnitOverlay(uiElementFactory),
                RightClickMenu.Create(uiElementFactory));
        }
    }
}
