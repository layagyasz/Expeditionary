using Cardamom.Graphics.Camera;
using Cardamom.Ui;
using Expeditionary.Controller.Scenes.Galaxies;
using Expeditionary.Model.Missions;

namespace Expeditionary.View.Scenes.Galaxies
{
    public class MissionLayerFactory
    {
        private readonly UiElementFactory _uiElementFactory;

        public MissionLayerFactory(UiElementFactory uiElementFactory)
        {
            _uiElementFactory = uiElementFactory;
        }

        public MissionLayer Create(MissionManager manager, ICamera camera)
        {
            return new(new MissionLayerController(manager, camera), _uiElementFactory);
        }
    }
}
