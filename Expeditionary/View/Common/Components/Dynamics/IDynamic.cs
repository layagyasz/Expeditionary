using Cardamom.Graphics;

namespace Expeditionary.View.Common.Components.Dynamics
{
    public interface IDynamic : IRenderable
    {
        EventHandler<EventArgs>? Refreshed { get; set; }
        void Refresh();
    }
}
