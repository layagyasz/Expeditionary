using Cardamom.Graphics;
using Cardamom.Ui.Controller;

namespace Expeditionary.View.Screens
{
    public interface IScreen : IRenderable
    {
        IController Controller { get; }
    }
}
