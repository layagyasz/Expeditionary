using Cardamom.Ui;
using Expeditionary.Model.Units;
using OpenTK.Mathematics;

namespace Expeditionary.Controller
{
    public record class AssetClickedEventArgs(
        Vector3i Position, IList<IAsset> Assets, MouseButtonClickEventArgs Button);
}
