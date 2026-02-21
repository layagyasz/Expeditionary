using Cardamom.Graphics;

namespace Expeditionary.View.Common.Components.Dynamics
{
    public interface IDynamic : IRenderable
    {
        event EventHandler<EventArgs>? Refreshed;
        void Refresh();
    }
}
