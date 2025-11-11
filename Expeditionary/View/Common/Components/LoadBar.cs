using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Controller;
using Cardamom.Ui;
using Expeditionary.Loader;
using Expeditionary.View.Common.Components.Dynamics;

namespace Expeditionary.View.Common.Components
{
    public class LoadBar : DynamicUiCompoundComponent
    {
        private static readonly string s_Container = "load-bar-outline";
        private static readonly string s_Bar = "load-bar-fill";
        private static readonly string s_Text = "load-bar-text";

        private LoadBar(IController controller, IUiContainer container, IUiElement bar, IUiElement text)
            : base(controller, container)
        {
            Add(bar);
            Add(text);
        }

        public static LoadBar Create(UiElementFactory uiElementFactory, LoaderStatus loaderStatus)
        {
            return new(
                new NoOpController(), 
                new DynamicUiContainer(
                      uiElementFactory.GetClass(s_Container),
                      new NoOpElementController()),
                new PoolBar(uiElementFactory.GetClass(s_Bar), new NoOpElementController(), loaderStatus.Progress),
                new DynamicTextUiElement(
                    uiElementFactory.GetClass(s_Text),
                    new NoOpElementController(),
                    () => loaderStatus.Progress.PercentFull().ToString("P0")));
        }
    }
}
