using Cardamom.Ui.Controller.Element;
using Cardamom.Ui;
using Cardamom.Ui.Elements;

namespace Expeditionary.View.Common.Components.Dynamics
{
    public class DynamicUiSerialContainer : UiSerialContainer, IDynamic
    {
        public EventHandler<EventArgs>? Refreshed { get; set; }

        public DynamicUiSerialContainer(Class @class, IElementController controller, Orientation orientation)
            : base(@class, controller, orientation) { }

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
