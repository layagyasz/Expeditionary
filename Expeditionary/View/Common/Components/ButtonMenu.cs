using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using Expeditionary.Controller.Common;

namespace Expeditionary.View.Common.Components
{
    public class ButtonMenu : UiCompoundComponent
    {
        private ButtonMenu(IController controller, IUiContainer container) 
            : base(controller, container) { }

        public class Builder
        {
            private readonly string _containerClass;
            private UiSerialContainer.Orientation _orientation = UiSerialContainer.Orientation.Vertical;
            private string? _title;
            private string? _titleClass;
            private readonly List<(string, SelectOption<object>)> _options = new();

            public Builder(string containerClass)
            {
                _containerClass = containerClass;
            }

            public Builder Add(string @class, string text, object value)
            {
                _options.Add((@class, SelectOption<object>.Create(value, text)));
                return this;
            }

            public Builder Horizontal()
            {
                _orientation = UiSerialContainer.Orientation.Horizontal;
                return this;
            }

            public Builder SetTitle(string title, string @class)
            {
                _title = title;
                _titleClass = @class;
                return this;
            }

            public ButtonMenu Build(UiElementFactory uiElementFactory)
            {
                var container = 
                    new UiSerialContainer(
                        uiElementFactory.GetClass(_containerClass), 
                        new TableController(uiElementFactory.GetAudioPlayer(), 0f), 
                        _orientation);
                if (_title != null && _titleClass != null)
                {
                    container.Add(
                        new TextUiElement(
                            uiElementFactory.GetClass(_titleClass),
                            new InlayController(uiElementFactory.GetAudioPlayer()),
                            _title));
                }
                foreach ((var @class, var option) in _options)
                {
                    container.Add(
                        new TextUiElement(
                            uiElementFactory.GetClass(@class),
                            new OptionElementController<object>(uiElementFactory.GetAudioPlayer(), option.Value),
                            option.Text));
                }
                return new(new ButtonMenuController(), container);
            }
        }
    }
}
