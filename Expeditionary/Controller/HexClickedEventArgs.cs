using Cardamom.Ui;
using OpenTK.Mathematics;

namespace Expeditionary.Controller
{
    public record class HexClickedEventArgs(Vector3i Hex, MouseButtonClickEventArgs Button);
}
