using OpenTK.Mathematics;

namespace Expeditionary.Model.Knowledge
{
    public interface IMapDiscovery
    {
        bool Discover(Vector2i offset);
        bool IsDiscovered(Vector2i offset);
    }
}
