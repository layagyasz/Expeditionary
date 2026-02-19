using Cardamom.Collections;
using Cardamom.Graphics;
using Cardamom.Ui;
using Expeditionary.Loader;
using Expeditionary.Model;
using Expeditionary.Runners.GameStates;
using Expeditionary.View.Common.Components.Dynamics;
using Expeditionary.View.Screens;
using Expeditionary.View;
using Expeditionary.View.Common.Interceptors;
using Expeditionary.Events;

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
        private readonly Dictionary<Type, IGameState> _stateMap;

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
                    { typeof(IGameStateContext.GalaxyContext), new GalaxyState(_config, _module) },
                    { typeof(IGameStateContext.LoadContext), new LoadState(_loader) },
                    { typeof(IGameStateContext.MainMenuContext), new MainMenuState(_module) },
                    { typeof(IGameStateContext.MatchContext), new MatchState(_module) }
                };
        }

        public void Enter(IGameStateContext context)
        {
            var newState = _stateMap[context.GetType()];
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
            Enter(new IGameStateContext.MainMenuContext());
        }

        private void HandleStateChange(object? sender, IGameStateContext e)
        {
            Enter(e);
        }

        private IRenderable AddInterceptors(IScreen screen)
        {
            IRenderable result = screen;
            if (screen.Controller is IEventDispatchable dispatch)
            {
                result = new EventInterceptor(result, dispatch);
            }
            if (screen is IDynamic)
            {
                result = new DynamicInterceptor(result, s_RefreshTime);
            }
            return new LocalizationInterceptor(result, _localization);
        }
    }
}
