using Cardamom.Ui;
using Expeditionary.Model.Matches.Assets;
using OpenTK.Mathematics;

namespace Expeditionary.Controller.Scenes.Matches.Layers
{
    public record class AssetClickedEventArgs(
        Vector3i Position, IList<IMatchAsset> Assets, MouseButtonClickEventArgs Button);
}
