using Expeditionary.Hexagons;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Knowledge
{
    public class UnknownMapDiscovery : DenseHexGrid<bool>, IMapDiscovery
    {
        private UnknownMapDiscovery(Vector2i size) 
            : base(size) { }

        public static UnknownMapDiscovery Create(Vector2i size)
        {
            return new UnknownMapDiscovery(size);
        }

        public bool Discover(Vector3i hex)
        {
            if (!Contains(hex))
            {
                return false;
            }
            var discovered = IsDiscovered(hex);
            Set(hex, true);
            return !discovered;
        }

        public bool IsDiscovered(Vector3i hex)
        {
            return Get(hex);
        }
    }
}
