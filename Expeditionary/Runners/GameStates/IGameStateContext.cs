using Expeditionary.Ai;
using Expeditionary.Loader;
using Expeditionary.Model.Mapping.Appearance;
using Expeditionary.Model;
using Expeditionary.Model.Missions;

namespace Expeditionary.Runners.GameStates
{
    public interface IGameStateContext
    {
        public record class GalaxyContext(MissionManager MissionManager) : IGameStateContext;

        public record class LoadContext(
            LoaderStatus Status, LoaderTaskNode<IGameStateContext> Task) : IGameStateContext;

        public record class MainMenuContext(): IGameStateContext;

        public record class MatchContext(
            Player Player, Match Match, MapAppearance Appearance, AiManager AiManager) : IGameStateContext;
    }
}
