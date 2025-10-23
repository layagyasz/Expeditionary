using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Elements;

namespace Expeditionary.Controller.Common
{
    public class ButtonMenuController : DynamicComponentControllerBase
    {
        public EventHandler<object>? OptionClicked { get; set; }

        protected override void BindElement(IUiElement element)
        {
            foreach (var controller in GetControllers(element))
            {
                if (controller is IOptionController<object> option)
                {
                    option.Selected += HandleElementClicked;
                }
            }
        }

        protected override void UnbindElement(IUiElement element)
        {
            foreach (var controller in GetControllers(element))
            {
                if (controller is IOptionController<object> option)
                {
                    option.Selected -= HandleElementClicked;
                }
            }
        }

        private void HandleElementClicked(object? sender, EventArgs e)
        {
            var controller = sender as IOptionController<object>;
            OptionClicked?.Invoke(this, controller!.Key);
        }
        
        private static IEnumerable<IController> GetControllers(IUiElement element)
        {
            yield return element.Controller;
            if (element is UiCompoundComponent compound)
            {
                yield return compound.ComponentController;
            }
        }
    }
}
