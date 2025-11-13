using Cardamom.Ui.Controller;
using Expeditionary.Controller.Scenes.Galaxies;
using Expeditionary.Model.Galaxies;
using Expeditionary.Model.Missions;
using Expeditionary.View.Screens;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Expeditionary.Controller.Screens
{
    public class GalaxyScreenController : IController
    {
        public EventHandler<Mission>? Launched { get; set; }

        private readonly SectorNaming _naming;

        private GalaxyScreen? _screen;
        private MissionLayerController? _missionLayerController;
        private MissionPaneController? _missionPaneController;

        public GalaxyScreenController(SectorNaming naming)
        {
            _naming = naming;
        }

        public void Bind(object @object)
        {
            _screen = (GalaxyScreen)@object;
            _missionLayerController = (MissionLayerController)_screen.Scene!.Missions.GroupController;
            _missionLayerController.MissionSelected += HandleMissionSelected;
            _missionPaneController = (MissionPaneController)_screen.MissionPane!.ComponentController;
            _missionPaneController.Launched += HandleLaunch;
        }

        public void Unbind()
        {
            _missionPaneController!.Launched -= HandleLaunch;
            _missionPaneController = null;
            _missionLayerController!.MissionSelected -= HandleMissionSelected;
            _missionLayerController = null;
            _screen = null;
        }

        private void HandleMissionSelected(object? sender, GalaxyClickedEventArgs<Mission> e)
        {
            if (e.Args.Button == MouseButton.Button1)
            {
                _missionPaneController!.Open(e.Element, _naming);
                _screen!.MissionPane!.Position = new Vector3(e.Args.ScreenPosition.X, e.Args.ScreenPosition.Y, 0);
            }
        }

        private void HandleLaunch(object? sender, Mission e)
        {
            Launched?.Invoke(this, e);
        }
    }
}
