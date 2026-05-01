using Cardamom.Ui;
using Cardamom.Utils.Suppliers.Promises;
using Expeditionary.Controller.Screens;
using Expeditionary.Loader;
using Expeditionary.Model.Instances;
using Expeditionary.Model.Matches;
using Expeditionary.Model.Matches.Ai;
using Expeditionary.Model.Missions;
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
            var missionGenerator = new MissionGenerator(data.Module.Galaxy, module.MapEnvironmentGenerator);
            var missionNodes =
                module.Campaigns.Values
                    .SelectMany(campaign => campaign.Stages).SelectMany(node => node.MissionNodes).ToList();
            var missionNode = missionNodes[random.Next(missionNodes.Count)];
            var mission = missionNode.Content.Generate(
                missionNode, new(module.Galaxy, module.MapEnvironmentGenerator, random));
            Console.WriteLine($"{mission.Map.Environment.Key} {mission.Map.Environment.Name}");
            foreach (var trait in mission.Map.Environment.Traits)
            {
                Console.WriteLine(trait.Key);
            }
            var player = mission.Players.First().Player;
            var status = new LoaderStatus(0);
            var creationContext = new CreationContext(player, random, data.Config.IsDebug);
            (var match, var appearance) = mission.Create(status, creationContext).GetNow();
            var aiManager = new AiManager(match);
            var setupContext = 
                new SetupContext(
                    aiManager,
                    mission.Players
                        .Select(playerSetup => 
                            new PlayerSetupContext(
                                playerSetup.Player, 
                                playerSetup.Player == player, 
                                new IFormationProvider.RandomFormationProvider(
                                    new(module.FactionFormations, module.Formations))))
                        .ToImmutableList());
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
