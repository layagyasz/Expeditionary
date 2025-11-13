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

        public static (LoaderStatus, LoaderTaskNode<Result>) Create(Mission mission, Player player, int seed)
        {
            var random = new Random(seed);
            var status = new LoaderStatus(logLength: 1);
            return (status, 
                Create(status, mission, player, random)
                    .Map(x => Setup(status, mission, x.Item1, x.Item2, player, random).GetNow()));
        }

        private static LoaderTaskNode<(Match, MapAppearance)> Create(
            LoaderStatus status, Mission mission, Player player, Random random)
        {
            var content = mission.Content;
            var creationContext = new CreationContext(status, player, IsTest: true);
            var result = content.Create(random, creationContext);
            return result;
        }

        private static LoaderTaskNode<Result> Setup(
            LoaderStatus status, Mission mission, Match match, MapAppearance appearance, Player player, Random random)
        {
            var content = mission.Content;
            var aiManager = new AiManager(match, content.Players.Select(x => x.Player).Where(x => x != player));
            var setupContext = new SetupContext(status, random, new SerialIdGenerator(), aiManager);
            return content.Setup(match, setupContext).Map(match => new Result(player, match, appearance, aiManager));
        }
    }
}
