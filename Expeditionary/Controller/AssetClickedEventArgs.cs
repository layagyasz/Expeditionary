using Cardamom.Ui;
using Expeditionary.Model.Combat;

namespace Expeditionary.Controller
{
    public record class AssetClickedEventArgs(IList<IAsset> Assets, MouseButtonClickEventArgs Button);
}
