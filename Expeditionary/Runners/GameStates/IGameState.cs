using Expeditionary.View.Screens;

namespace Expeditionary.Runners.GameStates
{
    public interface IGameState
    {
        event EventHandler<IGameStateContext>? GameStateChanged;
        void Exit();
        IScreen Enter(object? context, ScreenFactory screenFactory);
    }
}
