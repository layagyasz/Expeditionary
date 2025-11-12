namespace Expeditionary.Runners.GameStates
{
    public record class GameStateChangedEventArgs(GameStateId State, object? Context);
}
