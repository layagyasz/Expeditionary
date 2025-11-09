using Cardamom.Graphics.Camera;
using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using Expeditionary.Model.Missions;
using Expeditionary.View.Scenes.Galaxies;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Expeditionary.Controller.Scenes.Galaxies
{
    public class MissionLayerController : IController
    {
        public EventHandler<Mission>? MissionSelected { get; set; }

        private readonly MissionManager _manager;
        private readonly ICamera _camera;

        private MissionLayer? _layer;

        public MissionLayerController(MissionManager manager, ICamera camera)
        {
            _manager = manager;
            _camera = camera;
        }

        public void Bind(object @object)
        {
            _layer = (MissionLayer?)@object;
            _layer!.ElementAdded += HandleElementAdded;
            _layer!.ElementRemoved += HandleElementRemoved;
            _manager.MissionAdded += HandleMissionAdded;
            _manager.MissionRemoved += HandleMissionRemoved;
            _camera.Changed += HandleCamera;

            foreach (var element in _layer)
            {
                HandleElementAdded(_layer, new(element));
            }
            foreach (var mission in _manager.Missions)
            {
                _layer!.Add(mission);
            }
        }

        public void Unbind()
        {
            _camera.Changed -= HandleCamera;
            _layer!.ElementAdded -= HandleElementAdded;
            _layer!.ElementRemoved -= HandleElementRemoved;
            _manager.MissionAdded -= HandleMissionAdded;
            _manager.MissionRemoved -= HandleMissionRemoved;
            _layer = null;
        }

        private void HandleCamera(object? sender, EventArgs e)
        {
            _layer!.Dirty();
        }

        private void HandleMissionAdded(object? sender, Mission e)
        {
            _layer!.Add(e);
        }

        private void HandleMissionRemoved(object? sender, Mission e)
        {
            _layer!.Remove(e);
        }

        private void HandleElementAdded(object? sender, ElementEventArgs e)
        {
            var element = (IUiElement)e.Element;
            element.Controller.Clicked += HandleClick;
        }

        private void HandleElementRemoved(object? sender, ElementEventArgs e)
        {
            var element = (IUiElement)e.Element;
            element.Controller.Clicked -= HandleClick;
        }

        private void HandleClick(object? sender, MouseButtonClickEventArgs e)
        {
            if (e.Button == MouseButton.Button1)
            {
                var controller = (OptionElementController<Mission>)sender!;
                MissionSelected?.Invoke(this, controller.Key);
            }
        }
    }
}
