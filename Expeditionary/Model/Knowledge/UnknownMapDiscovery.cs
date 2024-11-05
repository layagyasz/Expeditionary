using OpenTK.Mathematics;

namespace Expeditionary.Model.Knowledge
{
    public class UnknownMapDiscovery : IMapDiscovery
    {
        private readonly bool[,] _tiles;

        private UnknownMapDiscovery(bool[,] tiles)
        {
            _tiles = tiles;
        }

        public static UnknownMapDiscovery Create(Vector2i size)
        {
            var tiles = new bool[size.X, size.Y];
            for (int i = 0; i < size.X; ++i)
            {
                for (int j = 0; j < size.Y; ++j)
                {
                    tiles[i, j] = false;
                }
            }
            return new UnknownMapDiscovery(tiles);
        }

        public bool Discover(Vector2i offset)
        {
            var discovered = IsDiscovered(offset);
            _tiles[offset.X, offset.Y] = true;
            return !discovered;
        }

        public bool IsDiscovered(Vector2i offset)
        {
            return _tiles[offset.X, offset.Y];
        }
    }
}
