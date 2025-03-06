using OpenTK.Mathematics;

namespace Expeditionary.Model.Knowledge
{
    public class KnownMapDiscovery : IMapDiscovery
    {
        public bool Discover(Vector3i hex)
        {
            return false;
        }

        public bool IsDiscovered(Vector3i hex)
        {
            return true;
        }
    }
}
