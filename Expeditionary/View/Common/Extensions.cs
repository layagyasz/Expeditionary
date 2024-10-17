using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;

namespace Expeditionary.View.Common
{
    public static class Extensions
    {
        public static IUiElement CreateButton(
            this UiElementFactory uiElementFactory, ButtonStyle style, IElementController controller, string Text)
        {
            return new UiSerialContainer(
                uiElementFactory.GetClass(style.Container),
                controller,
                UiSerialContainer.Orientation.Horizontal)
            {
                new SimpleUiElement(uiElementFactory.GetClass(style.Image), new InlayController()),
                new TextUiElement(uiElementFactory.GetClass(style.Text), new InlayController(), Text)
            };
        }
    }
}
