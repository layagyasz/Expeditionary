using Cardamom.Audio;
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

        private readonly AudioPlayer? _audioPlayer;
        private readonly Class _optionClass;

        public RightClickMenu(
            IController controller, IUiContainer container, Class optionClass, AudioPlayer? audioPlayer)
            : base(controller, container)
        {
            _audioPlayer = audioPlayer;
            _optionClass = optionClass;
        }

        public static RightClickMenu Create(UiElementFactory uiElementFactory)
        {
            return new RightClickMenu(
                new RadioController<object>(/* isNullable=*/ true),
                new UiSerialContainer(
                    uiElementFactory.GetClass(s_Container),
                    new InlayController(uiElementFactory.GetAudioPlayer()),
                    UiSerialContainer.Orientation.Vertical),
                uiElementFactory.GetClass(s_Option),
                uiElementFactory.GetAudioPlayer());
        }

        public void Set(IEnumerable<SelectOption<object>> options)
        {
            Clear(/* dispose= */ true);
            foreach (var option in options)
            {
                var element =
                    new TextUiElement(
                        _optionClass, new OptionElementController<object>(_audioPlayer, option.Value), option.Text);
                element.Initialize();
                Add(element);
            }
        }
    }
}
