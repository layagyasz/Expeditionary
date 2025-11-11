using Cardamom.Graphics;
using Cardamom.Ui;
using Cardamom.Utils.Generators.Samplers;
using Expeditionary.Ai;
using Expeditionary.Controller.Screens;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Missions.Generator;
using Expeditionary.Model.Missions;
using Expeditionary.Model;
using Expeditionary.View.Scenes.Matches;
using Expeditionary.View.Scenes;
using Expeditionary.View.Screens;
using Expeditionary.View.Common.Components;
using System.Collections.Immutable;
using Expeditionary.Loader;

namespace Expeditionary.Runners
{
    public class RandomMissionRunner : UiRunner
    {
        public RandomMissionRunner(ProgramConfig config)
            : base(config) { }

        protected override IRenderable MakeRoot(
           ProgramData data, UiElementFactory uiElementFactory, SceneFactory sceneFactory, ThreadedLoader loader)
        {
            var module = data.Module;
            var random = new Random();
            var missionGenerator =
                new MissionGenerator(
                    data.Module.Galaxy, 
                    module.MapEnvironmentGenerator,
                    new(module.FactionFormations, module.Formations));
            var missionNode =
                new MissionNode()
                {
                    Environment = new RandomEnvironmentProvider()
                    {
                        Sectors = ImmutableList.Create(1, 2, 3, 4, 5)
                    },
                    Difficulty = new() { MissionDifficulty.Medium },
                    Scale = new() { MissionScale.Medium },
                    Attackers = new() { module.Factions["faction-sm"] },
                    Defenders = new() { module.Factions["faction-earth"] },
                    Frequency = 1f,
                    Cap = 1,
                    Duration = new NormalSampler(1, 0),
                    Content = 
                        new AssaultMissionGenerator()
                        {
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
                        }
                };
            var mission = missionGenerator.Generate(missionNode, 0, random.Next()).Content;
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
