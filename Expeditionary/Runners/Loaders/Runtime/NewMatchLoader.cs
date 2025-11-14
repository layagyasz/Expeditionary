using Cardamom.Utils.Suppliers.Promises;
using Expeditionary.Ai;
using Expeditionary.Loader;
using Expeditionary.Model;
using Expeditionary.Model.Mapping.Appearance;
using Expeditionary.Model.Missions;

namespace Expeditionary.Runners.Loaders.Runtime
{
    public static class NewMatchLoader
    {
        public record class Result(Player Player, Match Match, MapAppearance Appearance, AiManager AiManager);

        public static (LoaderStatus, LoaderTaskNode<Result>) Create(
            Mission mission, Player player, bool isTest, int seed)
        {
            var random = new Random(seed);
            var status = new LoaderStatus(logLength: 1);
            return (status,
                Setup(status, mission, Create(status, mission, player, random, isTest), player, random));
                    
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
            Random random)
        {
            var content = mission.Content;
            var match = matchTask.GetPromise().Map(x => x.Item1);
            var setupContext = match.Map(match => CreateContext(match, mission.Content, player, random));
            var setupTask = content.Setup(status, match, setupContext);
            return matchTask.Map(x => setupTask.GetNow())
                .Map(match => 
                    new Result(
                        player, match, matchTask.GetPromise().Map(x => x.Item2).Get(), setupContext.Get().AiManager));
        }

        private static SetupContext CreateContext(Match match, MissionContent mission, Player player, Random random)
        {
            var aiManager = new AiManager(match, mission.Players.Select(x => x.Player).Where(x => x != player));
            return new SetupContext(random, new SerialIdGenerator(), aiManager);
        }
    }
}
