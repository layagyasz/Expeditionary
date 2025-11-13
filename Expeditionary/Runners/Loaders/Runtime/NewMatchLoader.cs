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

        private static readonly object s_Generate = new();
        private static readonly object s_Setup = new();

        public static (LoaderStatus, LoaderTaskNode<Result>) Create(Mission mission, Player player, int seed)
        {
            var random = new Random(seed);
            var status = new LoaderStatus(new List<object>() { s_Generate, s_Setup }, logLength: 1);
            return (status, new SourceLoaderTask<(Match, MapAppearance)>(
                    () => Create(status, mission, player, random), isGL: true)
                .Map(x => Create(status, mission, x.Item1, x.Item2, player, random)));
        }

        private static (Match, MapAppearance) Create(LoaderStatus status, Mission mission, Player player, Random random)
        {
            status.AddWork(s_Generate, 1);
            var content = mission.Content;
            var creationContext = new CreationContext(player, IsTest: true);
            var result = content.Create(random, creationContext);
            status.DoWork(s_Generate);
            return result;
        }

        private static Result Create(
            LoaderStatus status, Mission mission, Match match, MapAppearance appearance, Player player, Random random)
        {
            status.AddWork(s_Setup, 1);
            var content = mission.Content;
            var aiManager = new AiManager(match, content.Players.Select(x => x.Player).Where(x => x != player));
            var setupContext = new SetupContext(random, new SerialIdGenerator(), aiManager);
            content.Setup(match, setupContext);
            var result = new Result(player, match, appearance, aiManager);
            status.DoWork(s_Setup);
            return result;
        }
    }
}
