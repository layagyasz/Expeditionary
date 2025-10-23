using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;

namespace Expeditionary.View.Common.Components
{
    public class RightClickMenu : Radio
    {
        private static readonly string s_Container = "right-click-menu-container";
        private static readonly string s_Option = "right-click-menu-option";

        private readonly Class _optionClass;

        public RightClickMenu(IController controller, IUiContainer container, Class optionClass)
            : base(controller, container)
        {
            _optionClass = optionClass;
        }

        public static RightClickMenu Create(UiElementFactory uiElementFactory)
        {
            return new RightClickMenu(
                new RadioController<object>(/* isNullable=*/ true),
                new UiSerialContainer(
                    uiElementFactory.GetClass(s_Container),
                    new InlayController(),
                    UiSerialContainer.Orientation.Vertical),
                uiElementFactory.GetClass(s_Option));
        }

        public void Set(IEnumerable<SelectOption<object>> options)
        {
            Clear(/* dispose= */ true);
            foreach (var option in options)
            {
                var element =
                    new TextUiElement(_optionClass, new OptionElementController<object>(option.Value), option.Text);
                element.Initialize();
                Add(element);
            }
        }
    }
}
