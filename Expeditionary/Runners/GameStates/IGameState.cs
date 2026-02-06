using Cardamom.Graphics;
using Expeditionary.View.Screens;

namespace Expeditionary.Runners.GameStates
{
    public interface IGameState
    {
        EventHandler<GameStateChangedEventArgs>? GameStateChanged { get; set; }
        GameStateId Id { get; }

        void Exit();
        IScreen Enter(object? context, ScreenFactory screenFactory);
    }
}
