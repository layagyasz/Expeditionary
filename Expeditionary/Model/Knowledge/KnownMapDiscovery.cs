using OpenTK.Mathematics;

namespace Expeditionary.Model.Knowledge
{
    public class KnownMapDiscovery : IMapDiscovery
    {
        public bool Discover(Vector2i offset)
        {
            return false;
        }

        public bool IsDiscovered(Vector2i offset)
        {
            return true;
        }
    }
}
