using Cardamom.Ui.Controller.Element;
using Cardamom.Ui;
using Cardamom.Ui.Elements;

namespace Expeditionary.View.Common.Components.Dynamics
{
    public class DynamicUiContainer : UiContainer, IDynamic
    {
        public event EventHandler<EventArgs>? Refreshed;

        public DynamicUiContainer(Class @class, IElementController controller)
            : base(@class, controller) { }

        public virtual void Refresh()
        {
            foreach (var element in this)
            {
                if (element is IDynamic dynamic)
                {
                    dynamic.Refresh();
                }
            }
            Refreshed?.Invoke(this, EventArgs.Empty);
        }
    }
}
