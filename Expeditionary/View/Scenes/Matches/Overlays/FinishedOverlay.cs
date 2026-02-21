using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using Expeditionary.Controller.Scenes.Matches.Overlays;
using OpenTK.Mathematics;

namespace Expeditionary.View.Scenes.Matches.Overlays
{
    public class FinishedOverlay : UiCompoundComponent
    {
        private const string ContainerClass = "finished-overlay-container";
        private const string TextClass = "finished-overlay-text";

        public TextUiElement Text { get; }

        private FinishedOverlay(IController controller, IUiContainer container, TextUiElement text)
            : base(controller, container)
        {
            Text = text;
        }

        public override void ResizeContext(Vector3 bounds)
        {
            base.ResizeContext(bounds);
            Position = 0.5f * (bounds - Size);
        }

        public static FinishedOverlay Create(UiElementFactory uiElementFactory, Localization localization)
        {
            var container = 
                new UiSerialContainer(
                    uiElementFactory.GetClass(ContainerClass),
                    new NoOpElementController(),
                    UiSerialContainer.Orientation.Vertical);
            var text =
                new TextUiElement(
                    uiElementFactory.GetClass(TextClass),
                    new InlayController(uiElementFactory.GetAudioPlayer()),
                    string.Empty);
            container.Add(text);
            return new FinishedOverlay(new FinishedOverlayController(localization), container, text);
        }
    }
}
