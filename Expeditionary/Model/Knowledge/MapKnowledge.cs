using Expeditionary.Hexagons;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Knowledge
{
    public class MapKnowledge
    {
        private readonly TileKnowledge[,] _tiles;

        public MapKnowledge(TileKnowledge[,] tiles) 
        {
            _tiles = tiles;
        }

        public static MapKnowledge Create(Vector2i size, bool isDiscovered)
        {
            var tiles = new TileKnowledge[size.X, size.Y];
            for (int i=0; i<size.X; ++i)
            {
                for (int j=0; j<size.Y; ++j)
                {
                    tiles[i, j] = new(isDiscovered, 0);
                }
            }
            return new MapKnowledge(tiles);
        }

        public TileKnowledge Get(Vector3i hex)
        {
            var coord = Cubic.HexagonalOffset.Instance.Project(hex);
            return _tiles[coord.X, coord.Y];
        }
    }
}
