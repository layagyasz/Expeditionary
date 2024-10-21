using Expeditionary.Model.Combat;
using OpenTK.Mathematics;

namespace Expeditionary.Model
{
    public record class AssetMovedEventArgs(IAsset Asset, Vector3i Origin, Vector3i Destination, Pathing.Path? path);
}
