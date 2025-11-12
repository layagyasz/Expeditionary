using Expeditionary.Loader;
using Expeditionary.Runners.GameStates;
using Expeditionary.Runners.Loaders.Runtime;

namespace Expeditionary.Runners
{
    public class GalaxyRunner : UiRunner
    {
        public GalaxyRunner(ProgramConfig config)
            : base(config) { }

        protected override void Handle(ProgramController controller)
        {
            var module = controller.GetModule();
            (var status, var task) = NewGalaxyLoader.Load(module, seed: 0);
            controller.Enter(
                GameStateId.Load,
                new LoadState.LoadContext(
                    GameStateId.GalaxyOverview,
                    status,
                    task.Map(x => (object?)new GalaxyState.GalaxyContext(x))));
        }
    }
}
