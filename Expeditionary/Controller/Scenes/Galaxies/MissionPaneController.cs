using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Expeditionary.Controller.Common;
using Expeditionary.Model.Galaxies;
using Expeditionary.Model.Instances;
using Expeditionary.View.Scenes.Galaxies;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Expeditionary.Controller.Scenes.Galaxies
{
    public class MissionPaneController : SimplePaneController
    {
        public EventHandler<InstanceMission>? Launched { get; set; }

        private MissionPane? _pane;
        private IElementController? _launch;
        private InstanceMission? _mission;

        public override void Bind(object @object)
        {
            base.Bind(@object);
            _pane = (MissionPane)@object!;
            _launch = _pane.LaunchButton.Controller;
            _launch.Clicked += HandleLaunch;
        }

        public override void Unbind()
        {
            base.Unbind();
            _pane = null;
            _launch!.Clicked -= HandleLaunch;
            _launch = null;
        }

        public void Open(InstanceMission mission, SectorNaming naming)
        {
            _mission = mission;
            _pane!.SetMission(mission, naming);
            _pane.Visible = true;
        }

        private void HandleLaunch(object? sender, MouseButtonClickEventArgs e)
        {
            if (e.Button == MouseButton.Left)
            {
                Launched?.Invoke(this, _mission!);
            }
        }
    }
}
