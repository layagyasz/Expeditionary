using Cardamom.Collections;
using Cardamom.Ui;
using Expeditionary.Loader;
using Expeditionary.Model;
using Expeditionary.Runners.GameStates;
using Expeditionary.View.Screens;

namespace Expeditionary.Runners
{
    public class ProgramController
    {
        private readonly UiWindow _window;
        private readonly ThreadedLoader _loader;
        private readonly ScreenFactory _screenFactory;
        private readonly GameModule _module;
        private readonly EnumMap<GameStateId, IGameState> _stateMap;

        private IGameState? _state;

        public ProgramController(
            UiWindow window, ThreadedLoader loader, ScreenFactory screenFactory, GameModule module)
        {
            _window = window;
            _loader = loader;
            _screenFactory = screenFactory;
            _module = module;

            _stateMap =
                new()
                {
                    { GameStateId.GalaxyOverview, new GalaxyState(_module) },
                    { GameStateId.Load, new LoadState(_loader) },
                    { GameStateId.MainMenu, new MainMenuState(_module) }
                };
        }

        public void Enter(GameStateId state, object? context)
        {
            var newState = _stateMap[state];
            _window.SetRoot(newState.Enter(context, _screenFactory));
            _state?.Exit();
            _state = newState;
        }

        public GameModule GetModule()
        {
            return _module;
        }

        public void Initialize()
        {
            foreach (var state in _stateMap.Values)
            {
                state.GameStateChanged += HandleStateChange;
            }
            Enter(GameStateId.MainMenu, null);
        }

        private void HandleStateChange(object? sender, GameStateChangedEventArgs e)
        {
            Enter(e.State, e.Context);
        }
    }
}
