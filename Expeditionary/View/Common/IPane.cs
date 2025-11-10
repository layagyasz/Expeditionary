using Cardamom.Ui;

namespace Expeditionary.View.Common
{
    public interface IPane : IUiComponent, IUiContainer
    {
        IUiElement CloseButton { get; }
    }
}
