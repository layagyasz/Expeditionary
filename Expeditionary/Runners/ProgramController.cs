using Cardamom.Collections;
using Cardamom.Graphics;
using Cardamom.Ui;
using Expeditionary.Loader;
using Expeditionary.Model;
using Expeditionary.Runners.GameStates;
using Expeditionary.View.Common.Components.Dynamics;
using Expeditionary.View.Common;
using Expeditionary.View.Screens;
using Expeditionary.View;

namespace Expeditionary.Runners
{
    public class ProgramController
    {
        private static readonly long s_RefreshTime = 100;

        private readonly ProgramConfig _config;
        private readonly UiWindow _window;
        private readonly ThreadedLoader _loader;
        private readonly ScreenFactory _screenFactory;
        private readonly GameModule _module;
        private readonly Localization _localization;
        private readonly EnumMap<GameStateId, IGameState> _stateMap;

        private IGameState? _state;

        public ProgramController(
            ProgramConfig config,
            UiWindow window, 
            ThreadedLoader loader,
            ScreenFactory screenFactory,
            GameModule module,
            Localization localization)
        {
            _config = config;
            _window = window;
            _loader = loader;
            _screenFactory = screenFactory;
            _module = module;
            _localization = localization;

            _stateMap =
                new()
                {
                    { GameStateId.GalaxyOverview, new GalaxyState(_config, _module) },
                    { GameStateId.Load, new LoadState(_loader) },
                    { GameStateId.MainMenu, new MainMenuState(_module) },
                    { GameStateId.Match, new MatchState(_module) }
                };
        }

        public void Enter(GameStateId state, object? context)
        {
            var newState = _stateMap[state];
            _window.SetRoot(AddInterceptors(newState.Enter(context, _screenFactory)));
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

        private IRenderable AddInterceptors(IRenderable screen)
        {
            if (screen is IDynamic)
            {
                screen = new DynamicInterceptor(screen, s_RefreshTime);
            }
            return new LocalizationInterceptor(screen, _localization);
        }
    }
}
