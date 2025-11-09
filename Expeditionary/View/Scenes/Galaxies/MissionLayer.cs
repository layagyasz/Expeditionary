using Cardamom.Graphics;
using Cardamom.Graphics.Camera;
using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using Expeditionary.Model.Missions;
using OpenTK.Mathematics;

namespace Expeditionary.View.Scenes.Galaxies
{
    public class MissionLayer : UiGroup, IDisposable
    {
        private static readonly string s_ButtonClass = "galaxy-mission-button";

        private class MissionButton : SimpleUiElement
        {
            public Vector3 Pin { get; }

            public MissionButton(Class @class, IElementController controller, Vector3 pin)
                : base(@class, controller)
            {
                Pin = pin;
            }
        }

        private readonly UiElementFactory _uiElementFactory;

        private Vector3 _bounds;
        private bool _dirty;

        public MissionLayer(IController controller, UiElementFactory uiElementFactory)
            : base(controller)
        {
            _uiElementFactory = uiElementFactory;
        }

        public void Add(Mission mission)
        {
            Add(
                new MissionButton(
                    _uiElementFactory.GetClass(s_ButtonClass),
                    new OptionElementController<Mission>(_uiElementFactory.GetAudioPlayer(), mission),
                    new(mission.Position.X, 0, mission.Position.Y)));
        }

        public void Dirty()
        {
            _dirty = true;
        }

        public void Remove(Mission mission)
        {
            Remove(_elements.First(x => ((OptionElementController<Mission>)x.Controller).Key == mission));
        }

        public override void ResizeContext(Vector3 bounds)
        {
            _bounds = bounds;
            Dirty();
        }

        public void UpdateFromCamera(ICamera camera)
        {
            if (_dirty)
            {
                var sceneProjection = camera.GetProjection();
                var uiProjection = GetProjection();
                uiProjection.Invert();
                var transform = camera.GetViewMatrix() * sceneProjection.Matrix * uiProjection;
                foreach (var element in _elements)
                {
                    if (element is MissionButton button)
                    {
                        var projected = new Vector4(button.Pin, 1) * transform;
                        button.Visible = projected.Z < 0;
                        button.Position = new Vector3(projected.Xyz / projected.W) - 0.5f * button.Size;
                    }
                }
                _dirty = false;
            }
        }

        private Matrix4 GetProjection()
        {
            return Matrix4.CreateOrthographicOffCenter(0, _bounds.X, _bounds.Y, 0, -10, 10);
        }
    }
}
