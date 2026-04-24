using Expeditionary.Loader;
using Expeditionary.Model.Mapping.Appearance;
using Expeditionary.Model.Matches;
using Expeditionary.Model.Matches.Ai;
using Expeditionary.Model.Instances;

namespace Expeditionary.Runners.GameStates
{
    public interface IGameStateContext
    {
        public record class GalaxyContext(GameInstance Instance) : IGameStateContext;

        public record class InstanceSetupContext() : IGameStateContext;

        public record class LoadContext(
            LoaderStatus Status, LoaderTaskNode<IGameStateContext> Task) : IGameStateContext;

        public record class MainMenuContext(): IGameStateContext;

        public record class MatchContext(
            MatchPlayer Player, Match Match, MapAppearance Appearance, AiManager AiManager) : IGameStateContext;
    }
}
