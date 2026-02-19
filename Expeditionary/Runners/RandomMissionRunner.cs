using Cardamom.Ui;
using Cardamom.Utils.Generators.Samplers;
using Cardamom.Utils.Suppliers.Promises;
using Expeditionary.Ai;
using Expeditionary.Controller.Screens;
using Expeditionary.Loader;
using Expeditionary.Model;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Missions;
using Expeditionary.Model.Missions.Generator;
using Expeditionary.View.Common.Interceptors;
using Expeditionary.View.Screens;
using System.Collections.Immutable;

namespace Expeditionary.Runners
{
    public class RandomMissionRunner : UiRunner
    {
        public RandomMissionRunner(ProgramConfig config)
            : base(config) { }

        protected override void Handle(
            ProgramData data, UiWindow window, ThreadedLoader loader, ScreenFactory screenFactory)
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
            var status = new LoaderStatus(0);
            var creationContext = new CreationContext(player, random, data.Config.IsDebug);
            (var match, var appearance) = mission.Create(status, creationContext).GetNow();
            var aiManager = new AiManager(match, mission.Players.Select(x => x.Player).Where(x => x != player));
            var setupContext = new SetupContext(random, new SerialIdGenerator(), aiManager);
            match = 
                mission.Setup(
                    status, 
                    ImmediatePromise<Match>.Of(match), 
                    ImmediatePromise<SetupContext>.Of(setupContext))
                .GetNow();
            match.Initialize();
            aiManager.Initialize();
            match.Step();

            var screen = screenFactory.CreateMatch(match, appearance, player);
            window.SetRoot(
                new DynamicInterceptor(new EventInterceptor(screen, (MatchController)screen.Controller), 500L));
        }
    }
}
