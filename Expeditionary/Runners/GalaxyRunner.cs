using Cardamom.Graphics;
using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Expeditionary.View.Scenes;
using Expeditionary.View.Screens;

namespace Expeditionary.Runners
{
    public class GalaxyRunner : UiRunner
    {
        public GalaxyRunner(ProgramConfig config)
            : base(config) { }

        protected override IRenderable MakeRoot(
            ProgramData data, UiElementFactory uiElementFactory, SceneFactory sceneFactory)
        {
            return new GalaxyScreen(new NoOpController(), sceneFactory.CreateGalaxy());
        }
    }
}
