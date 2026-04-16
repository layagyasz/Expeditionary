using Cardamom.Utils.Suppliers.Promises;
using Expeditionary.Loader;
using Expeditionary.Model;
using Expeditionary.Model.Formations.Generator;
using Expeditionary.Model.Mapping.Appearance;
using Expeditionary.Model.Matches;
using Expeditionary.Model.Matches.Ai;
using Expeditionary.Model.Missions;
using System.Collections.Immutable;

namespace Expeditionary.Runners.Loaders.Runtime
{
    public static class NewMatchLoader
    {
        public record class Result(Player Player, Match Match, MapAppearance Appearance, AiManager AiManager);

        public static (LoaderStatus, LoaderTaskNode<Result>) Create(
            Mission mission, Player player, FormationGenerator formationGenerator, bool isTest, int seed)
        {
            var random = new Random(seed);
            var status = new LoaderStatus(logLength: 1);
            return (status, 
                Setup(status, mission, Create(status, mission, player, random, isTest), player, formationGenerator));  
        }

        private static LoaderTaskNode<(Match, MapAppearance)> Create(
            LoaderStatus status, Mission mission, Player player, Random random, bool isTest)
        {
            var creationContext = new CreationContext(player, random, isTest);
            return mission.Content.Create(status, creationContext);
        }

        private static LoaderTaskNode<Result> Setup(
            LoaderStatus status, 
            Mission mission,
            LoaderTaskNode<(Match, MapAppearance)> matchTask,
            Player player,
            FormationGenerator formationGenerator)
        {
            var content = mission.Content;
            var match = matchTask.GetPromise().Map(x => x.Item1);
            var setupContext = match.Map(match => CreateContext(match, mission.Content, player, formationGenerator));
            var setupTask = content.Setup(status, match, setupContext);
            return matchTask.Map(x => setupTask.GetNow())
                .Map(match => 
                    new Result(
                        player, match, matchTask.GetPromise().Map(x => x.Item2).Get(), setupContext.Get().AiManager));
        }

        private static SetupContext CreateContext(Match match, MissionContent mission, Player player, FormationGenerator formationGenerator)
        {
            return new SetupContext(
                new AiManager(match), 
                mission.Players.Select(
                    playerSetup => new PlayerSetupContext(
                        playerSetup.Player,
                        playerSetup.Player == player, 
                        new IFormationProvider.RandomFormationProvider(formationGenerator))).ToImmutableList());
        }
    }
}
