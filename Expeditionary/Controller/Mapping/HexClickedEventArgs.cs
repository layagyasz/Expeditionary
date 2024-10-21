using Cardamom.Ui;
using OpenTK.Mathematics;

namespace Expeditionary.Controller.Mapping
{
    public record class HexClickedEventArgs(Vector3i Hex, MouseButtonClickEventArgs Button);
}
