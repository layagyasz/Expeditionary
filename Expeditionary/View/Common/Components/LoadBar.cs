using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Elements;
using Cardamom.Ui;
using Expeditionary.Loader;
using Expeditionary.View.Common.Components.Dynamics;

namespace Expeditionary.View.Common.Components
{
    public class LoadBar : DynamicUiCompoundComponent
    {
        private static readonly string s_Container = "load-container";
        private static readonly string s_Bar = "load-bar";

        private LoadBar(IController controller, IUiContainer container, IUiElement bar)
            : base(controller, container)
        {
            Add(bar);
        }

        public static LoadBar Create(UiElementFactory uiElementFactory, LoaderStatus loaderStatus)
        {
            return new(
                new NoOpController(), 
                new DynamicUiSerialContainer(
                      uiElementFactory.GetClass(s_Container),
                      new NoOpElementController(),
                      UiSerialContainer.Orientation.Vertical),
                new PoolBar(uiElementFactory.GetClass(s_Bar), new NoOpElementController(), loaderStatus.Progress));
        }
    }
}
