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
        private readonly SectorNaming _naming;

        private GalaxyScreen? _screen;
        private MissionLayerController? _missionController;

        public GalaxyScreenController(SectorNaming naming)
        {
            _naming = naming;
        }

        public void Bind(object @object)
        {
            _screen = (GalaxyScreen)@object;
            _missionController = (MissionLayerController)_screen.Scene!.Missions.GroupController;
            _missionController.MissionSelected += HandleMissionSelected;
        }

        public void Unbind()
        {
            _missionController!.MissionSelected -= HandleMissionSelected;
            _missionController = null;
            _screen = null;
        }

        private void HandleMissionSelected(object? sender, GalaxyClickedEventArgs<Mission> e)
        {
            if (e.Args.Button == MouseButton.Button1)
            {
                _screen!.MissionPane!.Visible = true;
                _screen!.MissionPane.Position = new Vector3(e.Args.ScreenPosition.X, e.Args.ScreenPosition.Y, 0);
                _screen!.MissionPane!.SetTitle(_naming.Name(e.Element.Content.Map.Environment));
            }
        }
    }
}
