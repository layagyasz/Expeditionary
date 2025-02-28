using Cardamom.Ui;
using Expeditionary.Model.Units;

namespace Expeditionary.Controller
{
    public record class AssetClickedEventArgs(IList<IAsset> Assets, MouseButtonClickEventArgs Button);
}
