using OpenTK.Mathematics;

namespace Expeditionary.Model.Knowledge
{
    public interface IMapDiscovery
    {
        bool Discover(Vector3i hex);
        bool IsDiscovered(Vector3i hex);
    }
}
