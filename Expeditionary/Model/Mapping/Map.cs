using Expeditionary.Coordinates;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Mapping
{
    public class Map
    {
        public int Width => _tiles.GetLength(0);
        public int Height => _tiles.GetLength(1);

        private readonly Tile[,] _tiles;

        private Map(Tile[,] tiles)
        {
            _tiles = tiles;
        }

        public Tile Get(Offset2i position)
        {
            return _tiles[position.X, position.Y];
        }

        public class Builder
        {
            private readonly Tile[,] _tiles;

            public Builder(Vector2i size)
            {
                _tiles = new Tile[size.X, size.Y];
            }

            public Builder Set(Offset2i position, Tile tile)
            {
                _tiles[position.X, position.Y] = tile;
                return this;
            }

            public Map Build()
            {
                return new Map(_tiles);
            }
        }
    }
}
