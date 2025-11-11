using Cardamom.Ui.Controller.Element;
using Cardamom.Ui;
using Cardamom.Ui.Elements;

namespace Expeditionary.View.Common.Components.Dynamics
{
    public class DynamicTextUiElement : TextUiElement, IDynamic
    {
        private readonly Func<string> _textFn;

        public DynamicTextUiElement(
            Class @class, IElementController controller, Func<string> textFn) : base(@class, controller, textFn())
        {
            _textFn = textFn;
        }

        public void Refresh()
        {
            SetText(_textFn());
        }
    }
}
