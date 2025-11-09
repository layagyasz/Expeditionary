using Cardamom.Graphics.Camera;
using Cardamom.Ui;
using Cardamom.Ui.Controller;

namespace Expeditionary.View.Scenes.Galaxies
{
    public class MissionLayerFactory
    {
        private readonly UiElementFactory _uiElementFactory;

        public MissionLayerFactory(UiElementFactory uiElementFactory)
        {
            _uiElementFactory = uiElementFactory;
        }

        public MissionLayer Create(ICamera camera)
        {
            return new(new NoOpController(), _uiElementFactory, camera);
        }
    }
}
