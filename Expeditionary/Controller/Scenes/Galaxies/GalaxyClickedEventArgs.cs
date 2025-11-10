using Cardamom.Ui;

namespace Expeditionary.Controller.Scenes.Galaxies
{
    public record class GalaxyClickedEventArgs<T>(T Element, MouseButtonClickEventArgs Args);
}
