using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Elements;

namespace Expeditionary.View.Common.Components.Dynamics
{
    public class DynamicUiCompoundComponent : UiCompoundComponent, IDynamic
    {
        public EventHandler<EventArgs>? Refreshed { get; set; }

        public DynamicUiCompoundComponent(IController componentController, IUiContainer container)
            : base(componentController, container) { }

        public virtual void Refresh()
        {
            if (_container is IDynamic dynamic)
            {
                dynamic.Refresh();
            }
            Refreshed?.Invoke(this, EventArgs.Empty);
        }
    }
}
